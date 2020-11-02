using FeederNetInspcetor.Model;
using FeederNetInspector.Classes;
using System;
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
        }

        public void HandleOnTextChangedHostName(object sender, TextChangedEventArgs e)
        {
            hostName = tbHostName.Text;
        }

        public void SetTbRequests(RequestSessionModel requestSessionModel)
        {
            // Clear list
            lvRequest.Items.Clear();
            lvRequestIndicator.Items.Clear();

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
                string[] requestBodyLines = request.RequestBody.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (string line in requestBodyLines)
                {
                    lvRequest.Items.Add(line);
                }
            }

            // lvRequestIndicator
            int lvRequestItemHeight = (int)lvRequestIndicator.ActualHeight / lvRequest.Items.Count;
            for (int i = 0; i < lvRequest.Items.Count; i++)
            {
                if (i == lvRequest.Items.Count / 2)
                {
                    ListViewItem lviIndicator = new ListViewItem
                    {
                        Content = "",
                        Background = Brushes.Yellow,
                        Height = lvRequestItemHeight,
                    };
                    lvRequestIndicator.Items.Add(lviIndicator);
                }
                else
                {
                    lvRequestIndicator.Items.Add(new ListViewItem
                    {
                        Content = "",
                        Height = lvRequestItemHeight,
                        Visibility = Visibility.Hidden,
                    });
                }
            }

        }

        public void SetTbResponses(ResponseSessionModel responseSessionModel)
        {
            //tbResponse.Text = responseSessionModel.ResponseBody;
        }

        private void LvRequestIndicator_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int index = ((Selector)sender).SelectedIndex;
            ListView lv = (ListView)sender;
            ListViewItem lvItem = (ListViewItem)lv.SelectedItem;
            if (lvItem != null)
            {
                lvItem.IsSelected = false;
            }
            int scrollIndex = index != -1 && index < lvRequest.Items.Count - 1 ? index : lvRequest.Items.Count - 1;
            lvRequest.ScrollIntoView(lvRequest.Items[scrollIndex]);
        }
    }
}
