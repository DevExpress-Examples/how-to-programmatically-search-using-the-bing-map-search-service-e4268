﻿using DevExpress.Xpf.Map;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace XpfMapCustomSearchPanel {
    public partial class MainPage : UserControl {
        string keywords;
        string location;
        double latitude;
        double longitude;
        int startingIndex;

        public MainPage() {
            InitializeComponent();
        }

        private void GetSearchArguments() {
            keywords = tbKeywords.Text;
            location = tbLocation.Text;

            latitude = string.IsNullOrEmpty(tbLatitude.Text) ? 0 : double.Parse(tbLatitude.Text);
            longitude = string.IsNullOrEmpty(tbLongitude.Text) ? 0 : double.Parse(tbLongitude.Text);

            startingIndex = string.IsNullOrEmpty(tbStartingIndex.Text) ? 0 : int.Parse(tbLongitude.Text);
        }

        #region #Search
        private void Search() {
            searchDataProvider.Search(keywords, location, new GeoPoint(latitude, longitude), startingIndex);
        }
        #endregion #Search

        private void bSearch_Click(object sender, RoutedEventArgs e) {
            GetSearchArguments();
            Search();
        }

        #region #SearchCompleted
        private void searchDataProvider_SearchCompleted(object sender, BingSearchCompletedEventArgs e) {
            if (e.Error != null || e.Cancelled)
                return;

            SearchRequestResult result = e.RequestResult;
            if (result.ResultCode == RequestResultCode.Success) {
                LocationInformation region = result.SearchRegion;

                if (region != null) {
                    NavigateTo(region.Location);
                }

                else {
                    if (result.SearchResults.Count > 0)
                        NavigateTo(result.SearchResults[0].Location);
                }
                
                DisplayResults(e.RequestResult);
            }
        }

        private void NavigateTo(GeoPoint location) {
            mapControl.CenterPoint = location;
            mapControl.ZoomLevel = 7;
        }
        #endregion #SearchCompleted

        #region #DisplayResults
        private void DisplayResults(SearchRequestResult requestResult) {
            StringBuilder resultList = new StringBuilder("");
            resultList.Append(String.Format("Result Code: {0}\n", requestResult.ResultCode));
            resultList.Append(String.Format("Fault Reason: {0}\n", requestResult.FaultReason));
            resultList.Append(String.Format("Estimated Matches: {0}\n", requestResult.EstimatedMatches));
            resultList.Append(String.Format("Keyword: {0}\n", requestResult.Keyword));
            resultList.Append(String.Format("Location: {0}\n", requestResult.Location));
            resultList.Append(String.Format("\n"));

            if (requestResult.ResultCode == RequestResultCode.Success) {
                int resCounter = 1;
                foreach (LocationInformation resultInfo in requestResult.SearchResults) {
                    resultList.Append(String.Format("Result {0}:\n", resCounter));
                    resultList.Append(String.Format("Display Name: {0}\n", resultInfo.DisplayName));
                    resultList.Append(String.Format("Entity Type: {0}\n", resultInfo.EntityType));
                    resultList.Append(String.Format("Address: {0}\n", resultInfo.Address));
                    resultList.Append(String.Format("Location: {0}\n", resultInfo.Location));
                    resultList.Append(String.Format("______________________________\n"));

                    resCounter++;
                }
                if (requestResult.SearchRegion != null) {
                    resultList.Append(String.Format("\n===================================\n"));
                    resultList.Append(String.Format("Search region:\n"));
                    resultList.Append(String.Format("Display Name: {0}\n", requestResult.SearchRegion.DisplayName));
                    resultList.Append(String.Format("Entity Type: {0}\n", requestResult.SearchRegion.EntityType));
                    resultList.Append(String.Format("Address: {0}\n", requestResult.SearchRegion.Address));
                    resultList.Append(String.Format("Location: {0}\n", requestResult.SearchRegion.Location));
                }
                resultList.Append(String.Format("\n===================================\n"));
                resultList.Append(String.Format("Alternate search regions:\n\n"));
                resCounter = 1;
                foreach (LocationInformation locationInfo in requestResult.AlternateSearchRegions) {
                    resultList.Append(String.Format("Region {0}:\n", resCounter));
                    resultList.Append(String.Format("Display Name: {0}\n", locationInfo.DisplayName));
                    resultList.Append(String.Format("Entity Type: {0}\n", locationInfo.EntityType));
                    resultList.Append(String.Format("Address: {0}\n", locationInfo.Address));
                    resultList.Append(String.Format("Location: {0}\n", locationInfo.Location));
                    resultList.Append(String.Format("______________________________\n"));
                    resCounter++;
                }
            }

            tbResult.Text = resultList.ToString();
        }
        #endregion #DisplayResults
    }

}
