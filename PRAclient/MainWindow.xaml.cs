using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using PRAclient.Models;
using PRAclient.Views;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Controls.DataVisualization.Charting;

namespace PRAclient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RunAsync().GetAwaiter().GetResult();
            PageChangeSetup();
            LoadWindow(0);

        }

       public List<string> myEntities = new List<string>()
        { "Movie", "Contract", "CrewMember", "Position", "Studio" };


        public void LoadWindow(int ind)
        {   entityPickerCombo.Items.Clear();
            foreach (var x in myEntities)
            { entityPickerCombo.Items.Add(x); }
            addField1.Text= "";
            addField2.Text= "";
            addField3.Text= "";
            addField4.Text= "";
            addField1.Visibility = System.Windows.Visibility.Hidden; 
            addField2.Visibility = System.Windows.Visibility.Hidden; 
            addField3.Visibility = System.Windows.Visibility.Hidden; 
            addField4.Visibility = System.Windows.Visibility.Hidden;
            addItemBlock1.Visibility = System.Windows.Visibility.Hidden; 
            addItemBlock2.Visibility = System.Windows.Visibility.Hidden; 
            addItemBlock3.Visibility = System.Windows.Visibility.Hidden;
            addItemBlock4.Visibility = System.Windows.Visibility.Hidden;
            addItemBlock5.Visibility = System.Windows.Visibility.Hidden;
            addItemBlock6.Visibility = System.Windows.Visibility.Hidden;
            addCombo1.Visibility = System.Windows.Visibility.Hidden;
            addCombo2.Visibility = System.Windows.Visibility.Hidden;
            entityPickerCombo.SelectedIndex = ind;
            chartSwapper.IsEnabled = false;
            myChart.Visibility = System.Windows.Visibility.Hidden;
            UpdateStats();


        }
        static HttpClient client = new HttpClient();


        public async void UpdateStats()
        {
            try
            {
                List<int> obj = new List<int>();
                HttpResponseMessage response = await client.GetAsync("api/stats");
                if (response.IsSuccessStatusCode)
                {
                    obj = await response.Content.ReadAsAsync<List<int>>();
                }
                statBlock.Text = $"Database contains: {obj[0]} Movies, {obj[1]} Contracts," +
                    $" {obj[2]} Crew Members, {obj[3]} Positions, {obj[4]} Studios";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message.Trim()); }

        }

        static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("http://localhost:54940/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

        }

      
        static async Task<Uri> CreateItemAsync(object obj,string ent)
        {
            HttpResponseMessage response =
                await client.PostAsJsonAsync($"api/{ent}", obj);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }


        //
        static async Task<T[]> GetEntityAsync<T>(string entity)
        {
            T[] crew = null;
            HttpResponseMessage response;
            response = await client.GetAsync($"api/{entity}");
            if (response.IsSuccessStatusCode)
            {
                crew = await response.Content.ReadAsAsync<T[]>();                
            }
            return crew;
        }



        static async Task<T[]> GetEntityPaged<T>(string entity,int pageSize, int pageNumber)
        {
            try
            {
                T[] crew = null;
                HttpResponseMessage response;
                response = await client.GetAsync($"api/{entity}/{pageSize}/{pageNumber}");
                if (response.IsSuccessStatusCode)
                {
                    crew = await response.Content.ReadAsAsync<T[]>();
                }
                return crew;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.Trim());
                return null;
            }

        }

        static async Task<HttpStatusCode> UpdateItemAsync(object obj,string entity,int id)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"api/{entity}/{id}", obj);
            response.EnsureSuccessStatusCode();            
            return response.StatusCode;
        }


        static async Task<HttpStatusCode> DeleteProductAsync(string entity, string id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/{entity}/{id}");
            return response.StatusCode;
        }
        

        /// <summary>
        /// 
        /// </summary>
        string currentView = "";
        string currentEntity = "";
        int currentPage = 1;
        int pageSize;
        string cellValue="";

        //Controls behaviour of datagrid pagecontrol buttons
        public void PageChangeSetup()
        {   if(currentPage<=1)
            prevPage.IsEnabled=false;
            else prevPage.IsEnabled = true;

        if (dataGrid.Items.Count ==0)
                nextPage.IsEnabled = false;
            else nextPage.IsEnabled = true;
        }


        public string GetSelectedCellValue()
        {
            try
            {
                DataGridCellInfo cellInfo = dataGrid.SelectedCells[0];
                if (cellInfo == null) return null;

                DataGridBoundColumn column = cellInfo.Column as DataGridBoundColumn;
                if (column == null) return null;

                FrameworkElement element = new FrameworkElement() { DataContext = cellInfo.Item };
                BindingOperations.SetBinding(element, TagProperty, column.Binding);

                return element.Tag.ToString();
            }
            catch
            {
                MessageBox.Show("Please select a valid field.");
                return null;
            }
        }

        private async void Row_MouseDoubleClickAsync(object sender, MouseButtonEventArgs e)
        {
            cellValue = GetSelectedCellValue();
            string path = $"api/{currentEntity}/{cellValue}";


            if (currentView == "FilmCrewView")
            {
                LoadWindow(2);
                FilmCrew obj = new FilmCrew();
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    obj = await response.Content.ReadAsAsync<FilmCrew>();
                }
                LoadCrewBoxesAsync();
                addField1.Text = obj.Firstname;
                addField2.Text = obj.Name;
                addField3.Text = obj.Age.ToString();            
                
              
            }
            if (currentView == "MovieView")
            {
                LoadWindow(0);
                Movie obj = new Movie();
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    obj = await response.Content.ReadAsAsync<Movie>();
                }
                LoadMovieBoxesAsync();
                addField1.Text = obj.Title;
                addField2.Text = obj.ProductionYear.ToString();
                addField3.Text = obj.Budget.ToString(); ;
                addField4.Text = obj.Genre;
            }
            if (currentView == "ContractView")
            {
                LoadWindow(1);
                Contract obj = new Contract();
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    obj = await response.Content.ReadAsAsync<Contract>();
                }
                LoadContractBoxesAsync();
                addField1.Text = obj.Duration.ToString();
                addField2.Text = obj.Salary.ToString();
              
            }
            if (currentView == "Studio")
            {
                LoadWindow(4);
                Studio obj = new Studio();
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    obj = await response.Content.ReadAsAsync<Studio>();
                }
                LoadStudioBoxes();
                addField1.Text = obj.Name;
                addField2.Text = obj.YearOfEstablishment.ToString();

            }
            if (currentView == "Position")
            {
                LoadWindow(3);
                Position obj = new Position();
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    obj = await response.Content.ReadAsAsync<Position>();
                }
                LoadPositionBoxes();
                addField1.Text = obj.PositionName;

            }        

        }               

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
      

        private async void showCrew_ClickAsync(object sender, RoutedEventArgs e)
        {//add try catch ...
            currentPage = 1;   
            currentView = "FilmCrewView";
            currentEntity = "filmcrews";

            FilmCrewView[] arr = new FilmCrewView[0];
            arr=  await GetEntityPaged<FilmCrewView>("filmcrews", pageSize, currentPage);
            dataGrid.ItemsSource = arr;


            KeyValuePair<string, int>[] graphData = 
                new KeyValuePair<string, int>[arr.Length];            
                for (int i = 0; i < graphData.Length; i++)
                {
                   graphData[i]= new KeyValuePair<string, int>(arr[i].Position,arr[i].Age) ;
                }
           ((ColumnSeries)myChart.Series[0]).Title = "Age";
            ((ColumnSeries)myChart.Series[0]).ItemsSource = graphData;           

            PageChangeSetup();
            chartSwapper.IsEnabled = true;
        }


        private async void showContracts_ClickAsync(object sender, RoutedEventArgs e)
        {
            currentPage = 1;
            currentView = "ContractView";
            currentEntity = "contracts";
            ContractView[] arr = new ContractView[0];
            arr = await GetEntityPaged<ContractView>("contracts", pageSize, currentPage);
            dataGrid.ItemsSource = arr;


            KeyValuePair<string, int>[] graphData =
                new KeyValuePair<string, int>[arr.Length];
            for (int i = 0; i < graphData.Length; i++)
            {
                graphData[i] = 
                    new KeyValuePair<string, int>(arr[i].MovieTitle, arr[i].Salary);
            }
           ((ColumnSeries)myChart.Series[0]).Title = "Salary";
            ((ColumnSeries)myChart.Series[0]).ItemsSource = graphData;

            PageChangeSetup();
            chartSwapper.IsEnabled = true;
        }

        private async void showMovies_ClickAsync(object sender, RoutedEventArgs e)
        {
            currentPage = 1;
            currentView = "MovieView";
            currentEntity = "movies";
            MovieView[] arr = new MovieView[0];
            arr = await GetEntityPaged<MovieView>("movies", pageSize, currentPage);
            dataGrid.ItemsSource = arr;


            KeyValuePair<string, decimal>[] graphData =
                new KeyValuePair<string, decimal>[arr.Length];
            for (int i = 0; i < graphData.Length; i++)
            {
                graphData[i] =
                    new KeyValuePair<string, decimal>(arr[i].Title, arr[i].Budget);
            }
           ((ColumnSeries)myChart.Series[0]).Title = "Budget";
            ((ColumnSeries)myChart.Series[0]).ItemsSource = graphData;

            PageChangeSetup();
            chartSwapper.IsEnabled = true;

        }

        private async void showStudios_ClickAsync(object sender, RoutedEventArgs e)
        {
            currentPage = 1;
            currentView = "Studio";
            currentEntity = "studios";
            dataGrid.ItemsSource = 
                await GetEntityPaged<Studio>("studios", pageSize, currentPage);
            PageChangeSetup();

        }

        private async void showPositions_ClickAsync(object sender, RoutedEventArgs e)
        {
            currentPage = 1;
            PageChangeSetup();
            currentView = "Position";
            currentEntity = "positions";
            dataGrid.ItemsSource =
                await GetEntityPaged<Position>("positions", pageSize, currentPage);
            PageChangeSetup();

        }

        private void pageSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (pageSizeBox.Text != "")
            {
                try
                {
                    pageSize = Convert.ToInt32(pageSizeBox.Text);
                }
                catch
                {
                    MessageBox.Show("Only numbers in Int32 range are a valid input");
                    pageSizeBox.Text = "15";
                }
            }
        }

        private async void nextPage_ClickAsync(object sender, RoutedEventArgs e)
        {
            try {                
            currentPage++;
            PageChangeSetup();

            if (currentView == "FilmCrewView")
            {
                dataGrid.ItemsSource = await GetEntityPaged<FilmCrewView>(currentEntity, pageSize, currentPage);
            }
            if (currentView == "MovieView")
            {
                dataGrid.ItemsSource = await GetEntityPaged<MovieView>(currentEntity, pageSize, currentPage);
            }
            if (currentView == "ContractView")
            {
                dataGrid.ItemsSource = await GetEntityPaged<ContractView>(currentEntity, pageSize, currentPage);
            }
            if (currentView == "Studio")
            {
                dataGrid.ItemsSource = await GetEntityPaged<Studio>(currentEntity, pageSize, currentPage);
            }
            if (currentView == "Position")
            {
                dataGrid.ItemsSource = await GetEntityPaged<Position>(currentEntity, pageSize, currentPage);
            }
              }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.Trim());
            }
        }       

      

        private async void prevPage_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                currentPage--;
                PageChangeSetup();
                if (currentView == "FilmCrewView")
                {
                    dataGrid.ItemsSource = await GetEntityPaged<FilmCrewView>(currentEntity, pageSize, currentPage);
                }
                if (currentView == "MovieView")
                {
                    dataGrid.ItemsSource = await GetEntityPaged<MovieView>(currentEntity, pageSize, currentPage);
                }
                if (currentView == "ContractView")
                {
                    dataGrid.ItemsSource = await GetEntityPaged<ContractView>(currentEntity, pageSize, currentPage);
                }
                if (currentView == "Studio")
                {
                    dataGrid.ItemsSource = await GetEntityPaged<Studio>(currentEntity, pageSize, currentPage);
                }
                if (currentView == "Position")
                {
                    dataGrid.ItemsSource = await GetEntityPaged<Position>(currentEntity, pageSize, currentPage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.Trim());
            }
        }



        public async void LoadMovieBoxesAsync() {
            try
            {
                currentEntity = "movies";
                addField1.Visibility = System.Windows.Visibility.Visible;
                addField2.Visibility = System.Windows.Visibility.Visible;
                addField3.Visibility = System.Windows.Visibility.Visible;
                addField4.Visibility = System.Windows.Visibility.Visible;
                addItemBlock1.Visibility = System.Windows.Visibility.Visible;
                addItemBlock2.Visibility = System.Windows.Visibility.Visible;
                addItemBlock3.Visibility = System.Windows.Visibility.Visible;
                addItemBlock4.Visibility = System.Windows.Visibility.Visible;
                addItemBlock5.Visibility = System.Windows.Visibility.Visible;
                addCombo1.Visibility = System.Windows.Visibility.Visible;
                addItemBlock1.Text = "Title";
                addItemBlock2.Text = "Year ";
                addItemBlock3.Text = "Budget";
                addItemBlock4.Text = "Genre";
                addItemBlock5.Text = "Studio";
                addCombo1.Items.Clear();
                var studio = await GetEntityAsync<Studio>("studios");
                foreach (var x in studio)
                {
                    addCombo1.Items.Add(x.StudioId + ". " + x.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.Trim());
            }
        }
        public async void LoadContractBoxesAsync()
        {
           try { 
            currentEntity = "contracts";
            addField1.Visibility = System.Windows.Visibility.Visible;
            addField2.Visibility = System.Windows.Visibility.Visible;
            addItemBlock1.Visibility = System.Windows.Visibility.Visible;
            addItemBlock2.Visibility = System.Windows.Visibility.Visible;
            addItemBlock5.Visibility = System.Windows.Visibility.Visible;
            addItemBlock6.Visibility = System.Windows.Visibility.Visible;
            addCombo1.Visibility = System.Windows.Visibility.Visible;
            addCombo2.Visibility = System.Windows.Visibility.Visible;
            addItemBlock1.Text = "Duration";
            addItemBlock2.Text = "Salary ";
            addItemBlock5.Text = "CrewMember";
            addItemBlock6.Text = "Movie";
            addCombo1.Items.Clear();
            addCombo2.Items.Clear();
            var crew = await GetEntityAsync<FilmCrew>("filmcrews");
            addCombo1.Items.Clear();
            foreach (var x in crew)
            {
                addCombo1.Items.Add(x.CrewMemberId + ". " + x.Firstname + " " + x.Name);
            }
            var mov = await GetEntityAsync<Movie>("movies");
            foreach (var x in mov)
            {
                addCombo2.Items.Add(x.MovieId + ". " + x.Title);
            } }
             catch (Exception ex)
            {
                MessageBox.Show(ex.Message.Trim());
            }
        }

        public async void LoadCrewBoxesAsync()
        {
            try { 
            currentEntity = "filmcrews";
            addField1.Visibility = System.Windows.Visibility.Visible;
            addField2.Visibility = System.Windows.Visibility.Visible;
            addField3.Visibility = System.Windows.Visibility.Visible;
            addItemBlock1.Visibility = System.Windows.Visibility.Visible;
            addItemBlock2.Visibility = System.Windows.Visibility.Visible;
            addItemBlock3.Visibility = System.Windows.Visibility.Visible;
            addItemBlock5.Visibility = System.Windows.Visibility.Visible;
            addCombo1.Visibility = System.Windows.Visibility.Visible;
            addItemBlock1.Text = "Firstname";
            addItemBlock2.Text = "Surname ";
            addItemBlock3.Text = "Age";
            addItemBlock5.Text = "Position";
            addCombo1.Items.Clear();
            var pos = await GetEntityAsync<Position>("positions");
            foreach (var x in pos)
            {
                addCombo1.Items.Add(x.PositionId + ". " + x.PositionName);
            }
        } catch (Exception ex)
            {
                MessageBox.Show(ex.Message.Trim());
            }
}
        public  void LoadPositionBoxes()
        {   currentEntity = "positions";
            addField1.Visibility = System.Windows.Visibility.Visible;
            addItemBlock1.Visibility = System.Windows.Visibility.Visible;
            addItemBlock1.Text = "Position";
        }

        public void LoadStudioBoxes()

        {   currentEntity = "studios";
            addField1.Visibility = System.Windows.Visibility.Visible;
            addField2.Visibility = System.Windows.Visibility.Visible;
            addItemBlock1.Visibility = System.Windows.Visibility.Visible;
            addItemBlock2.Visibility = System.Windows.Visibility.Visible;
            addItemBlock1.Text = "Name";
            addItemBlock2.Text = "Year of Est.";
        }


            //add new data
        private async void addItem_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                if (entityPickerCombo.Text == myEntities[0])
                {
                    LoadWindow(0);
                    LoadMovieBoxesAsync();
                }

                if (entityPickerCombo.Text == myEntities[1])
                {
                    LoadWindow(1);
                    LoadContractBoxesAsync();
                    
                }
                if (entityPickerCombo.Text == myEntities[2])
                {
                    LoadWindow(2);
                    LoadCrewBoxesAsync();                    
                }

                if (entityPickerCombo.Text == myEntities[3])
                {
                    LoadWindow(3);
                    LoadPositionBoxes();              
                }

                if (entityPickerCombo.Text == myEntities[4])
                {
                    LoadWindow(4);
                    LoadStudioBoxes();
                   
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message.Trim()); }


        }

        private void entityPickerCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void addToDatabaseButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                if (entityPickerCombo.Text == myEntities[0])
                {
                    Movie obj = new Movie
                    {
                        MovieId = 0,
                        Title = addField1.Text,
                        ProductionYear = Convert.ToInt32(addField2.Text),
                        Budget = Convert.ToInt32(addField3.Text),
                        Genre = addField4.Text,
                        StudioId = Convert.ToInt32(addCombo1.Text.Split('.')[0])
                    };
                    await CreateItemAsync(obj, "movies");

                }
                if (entityPickerCombo.Text == myEntities[1])
                {
                    Contract obj = new Contract
                    {
                        ContractId = 1,
                        Duration = Convert.ToInt32(addField1.Text),
                        Salary = Convert.ToInt32(addField2.Text),
                        CrewMemberId = Convert.ToInt32(addCombo1.Text.Split('.')[0]),  ///
                        MovieId = Convert.ToInt32(addCombo2.Text.Split('.')[0])

                    };
                    await CreateItemAsync(obj, "contracts");

                }
                if (entityPickerCombo.Text == myEntities[2])
                {
                    FilmCrew obj = new FilmCrew
                    {
                        CrewMemberId = 0,
                        Name = addField1.Text,
                        Firstname = addField2.Text,
                        Age = Convert.ToInt32(addField3.Text),
                        PositionId = Convert.ToInt32(addCombo1.Text.Split('.')[0])
                    };
                    await CreateItemAsync(obj, "filmcrews");

                }
                if (entityPickerCombo.Text == myEntities[3])
                {
                    Position obj = new Position
                    {
                        PositionId = 0,
                        PositionName = addField1.Text
                    };
                    await CreateItemAsync(obj, "positions");

                }
                if (entityPickerCombo.Text == myEntities[4])
                {
                    Studio obj = new Studio
                    {
                        StudioId = 0,
                        Name = addField1.Text,
                        YearOfEstablishment = Convert.ToInt32(addField2.Text)
                    };
                    await CreateItemAsync(obj, "studios");
                }

                MessageBox.Show("Successfully added data to database!");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message.Trim()+
                " Failed to add data!"); }
            LoadWindow(0);
        }

        private async void deleteButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            try {
               await DeleteProductAsync(currentEntity, cellValue);
                MessageBox.Show("Item was successfully deleted!");
                }
            catch (Exception ex) { MessageBox.Show(ex.Message.Trim()); }
        }

        private async void updateItemButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                int cellValueInt = Convert.ToInt32(cellValue);

                if (entityPickerCombo.Text == myEntities[0])
                {
                    Movie obj = new Movie
                    {
                        MovieId = cellValueInt,
                        Title = addField1.Text,
                        ProductionYear = Convert.ToInt32(addField2.Text),
                        Budget = Convert.ToInt32(addField3.Text),
                        Genre = addField4.Text,
                        StudioId = Convert.ToInt32(addCombo1.Text.Split('.')[0])
                    };
                    await UpdateItemAsync(obj, "movies", cellValueInt);

                }
                if (entityPickerCombo.Text == myEntities[1])
                {
                    Contract obj = new Contract
                    {
                        ContractId = cellValueInt,
                        Duration = Convert.ToInt32(addField1.Text),
                        Salary = Convert.ToInt32(addField2.Text),
                        CrewMemberId = Convert.ToInt32(addCombo1.Text.Split('.')[0]),  ///
                        MovieId = Convert.ToInt32(addCombo2.Text.Split('.')[0])

                    };
                    await UpdateItemAsync(obj, "contracts",cellValueInt);

                }
                if (entityPickerCombo.Text == myEntities[2])
                {
                    FilmCrew obj = new FilmCrew
                    {
                        CrewMemberId = cellValueInt,
                        Name = addField1.Text,
                        Firstname = addField2.Text,
                        Age = Convert.ToInt32(addField3.Text),
                        PositionId = Convert.ToInt32(addCombo1.Text.Split('.')[0])
                    };
                    await UpdateItemAsync(obj, "filmcrews",cellValueInt);

                }
                if (entityPickerCombo.Text == myEntities[3])
                {
                    Position obj = new Position
                    {
                        PositionId = cellValueInt,
                        PositionName = addField1.Text
                    };
                    await UpdateItemAsync(obj, "positions",cellValueInt);

                }
                if (entityPickerCombo.Text == myEntities[4])
                {
                    Studio obj = new Studio
                    {
                        StudioId = cellValueInt,
                        Name = addField1.Text,
                        YearOfEstablishment = Convert.ToInt32(addField2.Text)
                    };
                    await UpdateItemAsync(obj, "studios",cellValueInt);
                }

                MessageBox.Show("Successfully updated data!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.Trim() +" Failed to update data!");
            }
            LoadWindow(0);
        }

        private void showOrHideChartButton_Click(object sender, RoutedEventArgs e)
        {
            if (myChart.Visibility == System.Windows.Visibility.Visible)
                myChart.Visibility = System.Windows.Visibility.Hidden;
            else myChart.Visibility = System.Windows.Visibility.Visible;

        }
    }

}

