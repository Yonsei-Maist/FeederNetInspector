/**
 * @file
 * @author Vicheka Phor, Yonsei Univ. Researcher, since 2020.10
 * @date 2020.11.02
 */
using FeederNetInspcetor.Model;
using FeederNetInspector.Classes;
using FeederNetInspector.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace FeederNetInspector.UI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Container : UserControl
    {
        private string hostName;

        public Container()
        {
            InitializeComponent();
            hostName = tbHostName.Text;
        }

        public string GetHostName()
        {
            return hostName;
        }

        public void Reset()
        {
            tbHostName.Text = "feedernet";
            lvRequest.Items.Clear();
            lvResponse.Items.Clear();
        }

        public void HandleOnTextChangedHostName(object sender, TextChangedEventArgs e)
        {
            hostName = tbHostName.Text;
        }

        public void SetTbRequests(RequestSessionModel requestSessionModel)
        {
            // Clear list
            lvRequest.Items.Clear();
            foreach (Request request in requestSessionModel.RequestList)
            {
                // Construct Request Headers
                ListViewItem lviRequestHeaderHeading = new ListViewItem
                {
                    Content = "Request Header",
                    FontSize = 12,
                    FontWeight = FontWeights.UltraBold,
                };
                lvRequest.Items.Add(lviRequestHeaderHeading);
                string[] requestHeaderLines = request.RequestHeaders.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (string line in requestHeaderLines)
                {
                    lvRequest.Items.Add(line);
                }
                // Construct Request Body
                ListViewItem lviRequestBodyHeading = new ListViewItem
                {
                    Content = "Request Body",
                    FontSize = 12,
                    FontWeight = FontWeights.UltraBold
                };
                lvRequest.Items.Add(lviRequestBodyHeading);
                string[] requestBodyLines = (JsonHelper.FormatJson(request.RequestBody)).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (string line in requestBodyLines)
                {
                    lvRequest.Items.Add(line);
                }
            }
        }

        public void SetTbResponses(ResponseSessionModel responseSessionModel)
        {
            // Clear list
            lvResponse.Items.Clear();
            lvResponseIndicator.Items.Clear();
            List<bool> responseIndicatorList = new List<bool>();
            
            foreach (Response response in responseSessionModel.ResponseList)
            {
                // Construct Response Headers
                ListViewItem lviResponseHeaderHeading = new ListViewItem
                {
                    Content = "Response Header",
                    FontSize = 12,
                    FontWeight = FontWeights.UltraBold,
                };
                lvResponse.Items.Add(lviResponseHeaderHeading);
                responseIndicatorList.Add(false);
                
                string[] responseHeaderLines = response.ResponseHeaders.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (string line in responseHeaderLines)
                {
                    lvResponse.Items.Add(line);
                    responseIndicatorList.Add(false);
                }
                // Construct Response Body
                ListViewItem lviResponseBodyHeading = new ListViewItem
                {
                    Content = "Response Body",
                    FontSize = 12,
                    FontWeight = FontWeights.UltraBold
                };
                lvResponse.Items.Add(lviResponseBodyHeading);
                responseIndicatorList.Add(false);

                string[] responseBodyLines = (JsonHelper.FormatJson(response.ResponseBody)).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (string line in responseBodyLines)
                {
                    bool isPersonalInformationExposed = false;
                    foreach (PersonalInformation pi in response.PersonalInformationList)
                    {
                        if (line.Contains(pi.Value))
                        {
                            isPersonalInformationExposed = true;
                            break;
                        }
                    }

                    if (isPersonalInformationExposed)
                    {
                        ListViewItem lvi = new ListViewItem
                        {
                            Content = line,
                            FontSize = 12,
                            Background = Brushes.Yellow,
                            FontWeight = FontWeights.UltraBold,
                        };
                        lvResponse.Items.Add(lvi);
                        responseIndicatorList.Add(true);
                    }
                    else
                    {
                        lvResponse.Items.Add(line);
                        responseIndicatorList.Add(false);
                    }
                }
            }

            // lvResponseIndicator
            int lvResponseIndicatorItemHeight = (int)lvResponseIndicator.ActualHeight / responseIndicatorList.Count;
            for (int i = 0; i < responseIndicatorList.Count; i++)
            {

                if (responseIndicatorList[i])
                {
                    lvResponseIndicator.Items.Add(new ListViewItem
                    {
                        Content = "",
                        Background = Brushes.Yellow,
                        Height = lvResponseIndicatorItemHeight,
                    });
                }
                else
                {
                    lvResponseIndicator.Items.Add(new ListViewItem
                    {
                        Content = "",
                        Height = lvResponseIndicatorItemHeight,
                        Visibility = Visibility.Hidden,
                        IsEnabled = false,
                    });
                }
            }

            // tbDetails
            string details = "";
            foreach (Response response in responseSessionModel.ResponseList)
            {
                foreach (PersonalInformation pi in response.PersonalInformationList)
                {
                    details += "key: " + pi.Key + "\n";
                    details += "value: " + pi.Value + "\n";
                }
            }
            tbDetails.Text = details;
        }

        private void LvResponseIndicator_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int index = ((Selector)sender).SelectedIndex;
            ListView lv = (ListView)sender;
            ListViewItem lvItem = (ListViewItem)lv.SelectedItem;
            if (lvItem != null)
            {
                lvItem.IsSelected = false;
            }
            int scrollIndex = index != -1 && index < lvResponse.Items.Count - 1 ? index : lvResponse.Items.Count - 1;
            lvResponse.ScrollIntoView(lvResponse.Items[scrollIndex]);
        }
    }
}
