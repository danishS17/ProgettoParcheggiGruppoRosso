using System.Windows;
using System.Data.SqlClient;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using System;
using System.Data;


namespace Parcheggi
{
  
    public partial class StoricoView : Window
    {
        SqlConnection connection = new SqlConnection("Data Source=DESKTOP-R6PQGP4\\SQLEXPRESSNEW;Initial Catalog=ParkingManagement;User ID=sa;Password=Danishveer&17012001");
        public StoricoView(string infoParkId)
        {
            InitializeComponent();
            BindDataGrid(infoParkId);
        }

        string infoParkId2 = "";
        private void BindDataGrid(string infoParkId)
        {
            
            SqlCommand cmd = new SqlCommand($"SELECT ParkingId, SearchDate, Stato, Revenue, VehicleId, Token FROM History WHERE InfoParkId = {infoParkId}", connection);

            infoParkId2 = infoParkId;

            connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            data.ItemsSource = dt.DefaultView;

            connection.Close();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime date = new DateTime();

            date = (DateTime)dateSelected.SelectedDate;

            connection.Open();

            SqlCommand cmd = new SqlCommand($"SELECT ParkingId, SearchDate, Stato, Revenue, VehicleId, Token FROM History WHERE InfoParkId = {infoParkId2} AND SearchDate = '{date}'", connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            data.ItemsSource = dt.DefaultView;

            connection.Close();
        }
    }
}
