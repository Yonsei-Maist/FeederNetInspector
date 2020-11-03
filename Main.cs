/**
 * @file
 * @author Vicheka Phor, Yonsei Univ. Researcher, since 2020.10
 * @date 2020.11.02
 */
using FeederNetInspcetor.Model;
using FeederNetInspector.Classes;
using FeederNetInspector.UI;
using FeederNetInspector.Utils;
using Fiddler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

[assembly: RequiredVersion("5.0.0.0")]

namespace FeederNetInspector
{
    public class Main : IAutoTamper //IFiddlerExtension //IAutoTamper
    {
        public static Container container;


        /// <summary>
        /// Called when Fiddler is shutting down
        /// </summary>
        public void OnBeforeUnload()
        {
            /*noop*/
        }

        /// <summary>
        /// Called when Fiddler User Interface is fully available
        /// </summary>
        public void OnLoad()
        {
            /* Load our UI: FeederNetInspector tab */
            TabPage page = new TabPage("FeederNetInspector");
            page.ImageIndex = (int)Fiddler.SessionIcons.Inspector;
            FiddlerApplication.UI.tabsViews.TabPages.Add(page);
            
            ElementHost element = new ElementHost();
            container = new Container();
            element.Child = container;
            element.Dock = DockStyle.Fill;

            page.Controls.Add(element);

            //Register an event handdler to CalculateReport event
            FiddlerApplication.CalculateReport += new CalculateReportHandler(DisplayResults);
        }

        /// <summary>
        /// An event handdler to summary the selected sessions
        /// Fire when user selects a session or switch tab
        /// If no session is selected, oSessions is an empty array
        /// </summary>
        private void DisplayResults(Session[] oSessions)
        {
            // If we're not at FeederNetInspector tab right now, reset our UI and bail out
            if (FiddlerApplication.UI.tabsViews.SelectedTab.Text != "FeederNetInspector")
            {
                container.Reset();
                if (oSessions.Length > 0)
                {
                    UnselectSessions(oSessions);
                }
                return;
            }
            // Filter sessions in the list view, keep only sessions that contain host name.
            FilterSessionListByHostName(container.GetHostName());

            try
            {
                if (oSessions.Length < 1)
                {
                    return;
                }
                Tuple<List<Request>, List<Response>> captureOutputTuple = GetRequestListResponseListAsTuple(oSessions);
                RequestSessionModel requestSessionModel = new RequestSessionModel(captureOutputTuple.Item1);
                ResponseSessionModel responseSessionModel = new ResponseSessionModel(captureOutputTuple.Item2);
                container.SetTbRequests(requestSessionModel);
                container.SetTbResponses(responseSessionModel);
            }
            catch (Exception e)
            {
                FiddlerApplication.Log.LogString(e.ToString());
                MessageBox.Show(e.Message, "Errors");
            }
        }

        /// <summary>
        /// Unselect sessions
        /// </summary>
        public static void UnselectSessions(Session[] oSessions)
        {
            foreach (Session oSession in oSessions)
            {
                oSession.ViewItem.Selected = false;
            }
        }

        /// <summary>
        /// Filter sessions in the list view, keep only sessions that contain host name
        /// </summary>
        private void FilterSessionListByHostName(string hostName)
        {
            ListView.ListViewItemCollection lvItems = FiddlerApplication.UI.lvSessions.Items;
            foreach (ListViewItem item in lvItems)
            {
                // 3 -> index of column host name
                if (!item.SubItems[3].Text.Contains(hostName))
                {
                    lvItems.Remove(item);
                }
            }
        }

        /// <summary>
        /// Get List<Request> and List</Response>
        /// </summary>
        /// <param name="oSessions"></param>
        /// <returns>Tuple<List<Request>, List<Response>></returns>
        private static Tuple<List<Request>, List<Response>> GetRequestListResponseListAsTuple(Session[] oSessions)
        {
            List<Request> requestList = new List<Request>();
            List<Response> responseList = new List<Response>();
            foreach (Session oSession in oSessions)
            {
                // Request
                string requestHeaders = oSession.RequestHeaders.ToString();
                string requestBody = oSession.GetRequestBodyAsString();
                List<PersonalInformation> personalInformationListInRequest = FindExposedPersonalInformation(oSession, oSession.GetRequestBodyAsString());
                Request request = new Request(requestHeaders, requestBody, personalInformationListInRequest);
                requestList.Add(request);

                // Response
                string responseHeaders = oSession.ResponseHeaders.ToString();
                string responseBody = oSession.GetResponseBodyAsString();
                List<PersonalInformation> personalInformationListInResponse = FindExposedPersonalInformation(oSession, oSession.GetResponseBodyAsString());
                Response response = new Response(responseHeaders, responseBody, personalInformationListInResponse);
                responseList.Add(response);

            }

            return new Tuple<List<Request>, List<Response>>(requestList, responseList);
        }

        /// <summary>
        /// Called before the user can edit a request using the Fiddler Inspectors
        /// </summary>
        public void AutoTamperRequestBefore(Session oSession)
        {
            /*noop*/
            // If we're not showing the FeederNetInspector tab right now, bail out.
            if (FiddlerApplication.UI.tabsViews.SelectedTab.Text != "FeederNetInspector")
            {
                return;
            }
            // begin filter
            // hide all request by default
            oSession["ui-hide"] = "true";
            string sessionHostName = oSession.hostname;
            string filterHostName = container.GetHostName();
            // list of processes to filter on
            string[] processlist = { "chrome", "iexplore", "microsoftedge", "msedge", "firefox", "torch" };
            for (var j = 0; j < processlist.Length; j++)
            {
                if (oSession.LocalProcess.Contains(processlist[j]))
                {
                    // Show item in web session list of Fiddler if sessionHostName contains filterHostName
                    if (sessionHostName.Contains(filterHostName))
                    {
                        oSession["ui-hide"] = null;
                    }
                }
            }
            // end filter
        }

        /// <summary>
        /// Find any exposed personal information
        /// </summary>
        /// <param name="oSession"></param>
        /// <param name="bodyAsString"></param>
        /// <returns>List<PersonalInformation> List of PersonalInformation</returns>
        private static List<PersonalInformation> FindExposedPersonalInformation(Session oSession, string bodyAsString)
        {
            List<PersonalInformation> personalInformationList = new List<PersonalInformation>();
            // check if contains personal information P.I
            if (bodyAsString != "")
            {
                MatchCollection mc = Regex.Matches(bodyAsString, "password\":\"([^\"]+)");
                foreach (Match m in mc)
                {
                    Console.WriteLine(m);
                    string passwordValue = m.Groups[1].Value;
                    bool isEncrypted = PasswordAdvisor.IsEncrypted(passwordValue);
                    // if not encrypted, password may be plain text so that we highlight the session
                    if (!isEncrypted)
                    {
                        PersonalInformation personalInformation = new PersonalInformation("password", passwordValue);
                        personalInformationList.Add(personalInformation);
                    }
                }
            }

            return personalInformationList;
        }

        /// <summary>
        /// Called after the user has had the chance to edit the request using the Fiddler Inspectors, but before the request is sent
        /// </summary>
        public void AutoTamperRequestAfter(Session oSession)
        {
            /*noop*/
         }

        /// <summary>
        /// Called before the user can edit a response using the Fiddler Inspectors, unless streaming.
        /// </summary>
        public void AutoTamperResponseBefore(Session oSession)
        {
            /*noop*/
        }

        /// <summary>
        /// Called after the user edited a response using the Fiddler Inspectors.  Not called when streaming.
        /// Process filter the session and high light the session if personal information (P.I) exposed
        /// Currently we check only weak and no excrpted password
        /// </summary>
        public void AutoTamperResponseAfter(Session oSession)
        {
            // If we're not showing the FeederNetInspector tab right now, bail out.
            if (FiddlerApplication.UI.tabsViews.SelectedTab.Text != "FeederNetInspector")
            {
                return;
            }
            // begin filter
            // hide all request by default
            oSession["ui-hide"] = "true";
            string sessionHostName = oSession.hostname;
            string filterHostName = container.GetHostName();
            // list of processes to filter on
            string[] processlist = { "chrome", "iexplore", "microsoftedge", "msedge", "firefox", "torch" };
            for (var j = 0; j < processlist.Length; j++)
            {
                if (oSession.LocalProcess.Contains(processlist[j]))
                {
                    // Show item in web session list of Fiddler if sessionHostName contains filterHostName
                    if (sessionHostName.Contains(filterHostName))
                    {
                        oSession["ui-hide"] = null;
                        //List<PersonalInformation> personalInformationListInRequest = FindExposedPersonalInformation(oSession, oSession.GetRequestBodyAsString());
                        List<PersonalInformation> personalInformationListInResponse = FindExposedPersonalInformation(oSession, oSession.GetResponseBodyAsString());
                        //bool isHighlight = personalInformationListInRequest.Count != 0 || personalInformationListInResponse.Count != 0;
                        bool isHighlight = personalInformationListInResponse.Count != 0;
                        if (isHighlight)
                        {
                            oSession.ViewItem.BackColor = Color.Yellow;
                        }
                    }
                }
            }
            // end filter
        }

        /// <summary>
        /// Called Fiddler returns a self-generated HTTP error (for instance DNS lookup failed, etc)
        /// </summary>
        public void OnBeforeReturningError(Session oSession)
        {
            /*noop*/
        }

    }

}