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
    public partial class MainWindow : Window
    {
        SqlConnection connection = new SqlConnection("Data Source=DESKTOP-R6PQGP4\\SQLEXPRESSNEW;Initial Catalog=ParkingManagement;User ID=sa;Password=Danishveer&17012001");
       
        Dictionary<string, Button> Bottoni = new Dictionary<string, Button>();
        public MainWindow()
        {
            InitializeComponent();

            SqlCommand cmd = new SqlCommand("SELECT * FROM InfoParking",connection);

            connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            //riempo il combobox
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
                Bottoni.Clear();
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

                    if(comboSelected)
                    {
                        connection.Open();

                        string checkState = $"SELECT Stato From Parking WHERE ParkingId = '{"P" + iRow.ToString() + jCol.ToString()}' AND InfoParkId = {combo.SelectedValue.ToString()}";
                        SqlCommand checkStateCommand = new SqlCommand(checkState,connection);
                        bool state = (bool)checkStateCommand.ExecuteScalar();

                        if(state)
                        {
                            B.Style = FindResource("VeicoloClick2") as Style;
                        }

                        connection.Close();
                    }

                    B.Click += ParcheggioClick;

                    Bottoni.Add("P" + iRow.ToString() + jCol.ToString(), B);

                    panel.Child = B;

                    DynamicGrid.Children.Add(panel);
                }

            }
  
        }

        string oldKey = "P00";
        bool parcheggiCheckState = false;
        string clickedButton = "P00";
        private void ParcheggioClick(object sender, RoutedEventArgs e)
        {
            string infoParkId = combo.SelectedValue.ToString();

            connection.Open();

            string checkStateCommand = "SELECT Stato FROM Parking WHERE InfoParkId = "+infoParkId+" AND ParkingId = '"+oldKey+"'";
            SqlCommand checkStateSQL = new SqlCommand(checkStateCommand, connection);
            bool state = (bool)checkStateSQL.ExecuteScalar();
            //MessageBox.Show(state.ToString());
            connection.Close();

            if (state)
            {
                Bottoni[oldKey].Style = FindResource("VeicoloClick2") as Style;
            }
            else
            {
                Bottoni[oldKey].Style = FindResource("StileVeicolo") as Style;
            }


            
            ((Button)sender).Style = FindResource("VeicoloClick") as Style;
            clickedButton = ((Button)sender).Name;
            oldKey = ((Button)sender).Name;
            parcheggiCheckState = true;

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



                }
            }
        }


        // string oldIndex = "0";

        bool comboSelected = false;
        private void ComboBoxSelection(object sender, SelectionChangedEventArgs e)
        {

            string index = combo.SelectedValue.ToString();
            DynamicGrid.RowDefinitions.Clear();    //cancello le righe
            DynamicGrid.ColumnDefinitions.Clear(); //cancello le colonne
            DynamicGrid.Children.Clear(); //cancello i componenti UI del grid
            Bottoni.Clear();

            comboSelected = true;

           connection.Open();

           string selectRow = $"SELECT Nrighe FROM InfoParking WHERE InfoParkId ={index}";
           SqlCommand comandrow = new SqlCommand(selectRow, connection);
           int row = (int)comandrow.ExecuteScalar();

           string selectCol = $"SELECT Ncol FROM InfoParking WHERE InfoParkId ={index}";
           SqlCommand comandcol = new SqlCommand(selectRow, connection);
           int col = (int)comandcol.ExecuteScalar();

           connection.Close();

           GeneraRigheDinamiche(row); //con questo metodo genero le righe del grid
           GeneraColDinamiche(col);   //con questo metodo genero le colonne del grid
           GeneraButton(); //con questo metodo genero i button

            //Button_Entra.Visibility = Visibility.Visible;

            Veicoli_metodi.IsEnabled = true;

            //ComboBoxItem typeItem = (ComboBoxItem)combo.SelectedItem;

            //  MessageBox.Show(row.ToString());

        }

        bool checkOccupation(string infoParkId, string ParkingId)
        {
        

            string check1 = $"SELECT Stato FROM Parking WHERE ParkingId = '{ParkingId}' AND InfoParkId = {infoParkId}";
            SqlCommand checkCommand = new SqlCommand(check1, connection);
            bool truth = (bool)checkCommand.ExecuteScalar();

            //MessageBox.Show(truth.ToString());

            if (truth)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void ButtonEntraClick(object sender, RoutedEventArgs e)
        {
            string targa = TargaText.Text;
            string infoParkId = combo.SelectedValue.ToString();
            connection.Open();

            string check = "IF EXISTS(SELECT Vehicle.LicensePlate FROM Vehicle WITH(NOLOCK) WHERE LicensePlate = '"+targa+"') BEGIN SELECT '1' END ELSE BEGIN SELECT '0' END";
            SqlCommand command = new SqlCommand(check,connection);
            string result = (string)command.ExecuteScalar();

            if (result == "0")
            {
                MessageBox.Show("Targa inserita non esiste oppure è sbagliata", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (parcheggiCheckState == false )
            {
                MessageBox.Show("Non hai selezionato un parcheggio", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (checkOccupation(infoParkId, clickedButton))
            {
                MessageBox.Show("Parcheggio già occupato", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Random random = new Random();

                
                //prima prendo l'id del veicolo SELECT VehicleID FROM Vehicle WHERE Vehicle.LicensePlate = 'Targa'

                //poi prendo prendo il valore del indice selezionato e faccio il UPDATE e genero il token
                //nel second step inserisco anche la data in entrata e quello di uscita la metto dopo.

                string sqlSelect = "SELECT VehicleID FROM Vehicle WHERE Vehicle.LicensePlate = '"+targa+"'";
                SqlCommand commandSelect = new SqlCommand(sqlSelect,connection);
                string VehicleId = commandSelect.ExecuteScalar().ToString();
                string token = random.Next(10000, 90000).ToString();
               

                string sqlUpate = "UPDATE Parking SET Stato = 1,Revenue = 0,VehicleId = "+VehicleId+" ,Token = '"+token+"', EntryTimeDate = '"+DateTime.Now+"' WHERE InfoParkId = '"+infoParkId+"' AND ParkingId = '"+clickedButton+"'";
                SqlCommand commandUpate = new SqlCommand(sqlUpate, connection);
                commandUpate.ExecuteNonQuery();

               // Bottoni[clickedButton].Content = targa;

                Bottoni[clickedButton].Style = FindResource("VeicoloClick2") as Style;

                //MessageBox.Show(combo.SelectedValue.ToString());

                MessageBox.Show(token);

            }

            connection.Close();

        }
        int Counter = 0;
        private void ButtonUscitaClick(object sender, RoutedEventArgs e)
        {
            string token = "";
            string infoParkId = combo.SelectedValue.ToString();

            Counter++;

            connection.Open();

            string SqlToken = $"SELECT Parking.Token FROM Parking WHERE InfoParkId = {infoParkId}  AND ParkingId = '{clickedButton}'";
            SqlCommand tokenCommand = new SqlCommand(SqlToken, connection);
            token = tokenCommand.ExecuteScalar().ToString();



            //MessageBox.Show(token);

            if (token != TargaText.Text)
            {
                MessageBox.Show("Inserire il token fornito!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {

                string SqlSelectRevenue = "SELECT Revenue FROM Parking WHERE InfoParkId = " + combo.SelectedValue + " AND ParkingId = '" + clickedButton + "'";
                SqlCommand SelectCommand = new SqlCommand(SqlSelectRevenue, connection);
                string revenue = (SelectCommand.ExecuteScalar()).ToString();

                string SqlSelectDateTime = "SELECT EntryTimeDate FROM Parking WHERE InfoParkId = " + combo.SelectedValue + " AND ParkingId = '" + clickedButton + "'";
                SqlCommand DateTimeCommand = new SqlCommand(SqlSelectRevenue, connection);
                string dateTime = Convert.ToString(SelectCommand.ExecuteScalar());

               // MessageBox.Show(dateTime);

                //imeSpan diff = DateTime.Now - dateTime;
               
               double calcola = Convert.ToDouble(Counter) * 2.00;

                string SqlUpdateCommand = $"UPDATE Parking SET Revenue = {calcola}, Stato = 0, EntryTimeDate = NULL, VehicleId = NULL, Token = NULL WHERE InfoParkId = {infoParkId} AND ParkingId = '{clickedButton}'";
                SqlCommand updateCommand = new SqlCommand(SqlUpdateCommand, connection);
                updateCommand.ExecuteNonQuery();

                Bottoni[clickedButton].Style = FindResource("StileVeicolo") as Style;

                MessageBox.Show(" La macchina è uscita");

            }

            connection.Close();

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            connection.Open();

            string SqlTranferDataCommand = "INSERT INTO History(ID,ParkingId,Stato,Revenue,EntryTimeDate,VehicleId,InfoParkId,Token) SELECT * FROM Parking";
            SqlCommand commandTransfer = new SqlCommand(SqlTranferDataCommand, connection);
            commandTransfer.ExecuteNonQuery();

            connection.Close();
        }

        private void ShowStoricoClick(object sender, RoutedEventArgs e)
        {
            StoricoView storicoView = new StoricoView(combo.SelectedValue.ToString());

            storicoView.Show();
        }
    }
}
