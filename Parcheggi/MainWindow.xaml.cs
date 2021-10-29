using System.Windows;
using System.Data.SqlClient;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using System.Data;

namespace Parcheggi
{
    public partial class MainWindow : Window
    {
        SqlConnection connection = new SqlConnection("Data Source=DESKTOP-R6PQGP4\\SQLEXPRESSNEW;Initial Catalog=ParkingManagement;User ID=sa;Password=Danishveer&17012001");
        public MainWindow()
        {
            InitializeComponent();


            SqlCommand cmd = new SqlCommand("SELECT * FROM InfoParking",connection);

            connection.Open();
            SqlCommand cmdDelete = new SqlCommand("delete Parking", connection);
            cmdDelete.ExecuteNonQuery();
            connection.Close();

            connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            combo.ItemsSource = dt.DefaultView;

            combo.DisplayMemberPath = "NamePark";
            combo.SelectedValuePath = "InfoParkId";

            connection.Close();

        }

        bool checkButtonState = true;


        private void ConfermaClick(object sender, RoutedEventArgs e)
        {
            if (checkButtonState == false)
            {
                GeneraGrid();
            }
            else
            {
                DynamicGrid.RowDefinitions.Clear();    //cancello le righe
                DynamicGrid.ColumnDefinitions.Clear(); //cancello le colonne
                DynamicGrid.Children.Clear(); //cancello i componenti UI del grid

                GeneraGrid(); //genero la nuova grid
            }
        }

        private void GeneraGrid()
        {
            int row, col;

            int.TryParse(Row.Text, out row);
            int.TryParse(Col.Text, out col);

            if (row <= 0 || col <= 0 || Nome.Text == "") //se utente non inserisce uno dei due dati(row o col) stampa il message di errore
            {
                MessageBox.Show("Non hai fornito uno dei dati richiesti(Row o Col)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                GeneraRigheDinamiche(row); //con questo metodo genero le righe del grid
                GeneraColDinamiche(col);   //con questo metodo genero le colonne del grid

                GeneraButton(); //con questo metodo genero i button
                CreateParking(row, col,Nome.Text);
                                // printDictionary();

                checkButtonState = true;
                Veicoli_metodi.IsEnabled = true;
            }
        }

        private void GeneraButton()
        {

            
            int iRow = -1;
            foreach (RowDefinition righe in DynamicGrid.RowDefinitions)
            {
                iRow++;
                int jCol = -1;

                foreach (ColumnDefinition colonne in DynamicGrid.ColumnDefinitions)
                {

                    jCol++;

                    //I pannelli sono uno dei tipi di controllo più importanti di WPF. Fungono da contenitori per altri controlli
                    //e controllano il layout delle finestre/pagine.

                    Border panel = new Border();
                    {
                        BorderThickness = new Thickness(1);
                        //BorderBrush = new SolidColorBrush(Colors.Red);

                    }

                    Grid.SetColumn(panel, jCol); //setto le colonne
                    Grid.SetRow(panel, iRow); //setto le righe


                    //genero il button
                    Button B = new Button()
                    {
                        Margin = new Thickness(3),
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,

                        Style = FindResource("StileVeicolo") as Style,
                        Name = "P" + iRow.ToString() + jCol.ToString(),     //nome del bottone ho messo nome del button uguale al contenuto 

                        Content = "P" + iRow.ToString() + jCol.ToString(), //assegno come il content il numero della cella che corrisponde alla righa e colonna in cui si trova
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Background = new SolidColorBrush(Colors.White),

                    };

                    panel.Child = B;

                    DynamicGrid.Children.Add(panel);
                }

            }
  
        }

        private void GeneraRigheDinamiche(int row) //Genera le righe in modo dinamiche
        {
            for (int i = 0; i < row; i++) //Con questo ciclo genero le righe
            {
                DynamicGrid.RowDefinitions.Add(new RowDefinition());

            }
        }

        private void GeneraColDinamiche(int col) //Genera le colonne in modo dinamiche
        {
            for (int i = 0; i < col; i++) //Con questo ciclo genere le colonne
            {
                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        private void CreateParking(int row, int col, string nameOfParking)
        {

            //inserisco nella tabella info parking i dati
            connection.Open();
            string commandInsert = "INSERT INTO InfoParking VALUES('"+nameOfParking+"',"+col+", "+row+")";
            SqlCommand commandIns = new SqlCommand(commandInsert,connection);
            commandIns.ExecuteNonQuery();
            connection.Close();

            //Prendo l'ultimo id

            connection.Open();
            string Select = "SELECT IDENT_CURRENT('InfoParking')";
            SqlCommand commandSelect = new SqlCommand(Select,connection);
            int currentId = Convert.ToInt32(commandSelect.ExecuteScalar()); //prendo l'id corrente
            connection.Close();

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {

                    //carico la tabella parking
                    connection.Open();

                    //currentId++;

                    string ParkingId = "P" + i.ToString() + j.ToString();
                    string commandInsertParking = "INSERT INTO Parking (ParkingId,Stato,InfoParkId) VALUES('"+ParkingId+"', 0,"+currentId+")";
                    SqlCommand commandInsParking = new SqlCommand(commandInsertParking, connection);
                    commandInsParking.ExecuteNonQuery();

                    connection.Close();


                    //carico la tabella storico
                    connection.Open();
                    

                    string ParkingIdStorico = "P" + i.ToString() + j.ToString();
                    string commandInsertStorico = "INSERT INTO History (ParkingId,Stato,InfoParkId) VALUES('" + ParkingIdStorico + "', 0," + currentId + ")";
                    SqlCommand commandInsStorico = new SqlCommand(commandInsertStorico, connection);
                    commandInsStorico.ExecuteNonQuery();

                    connection.Close();



                }
            }
        }





    }
}
