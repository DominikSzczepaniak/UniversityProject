using System.Collections.ObjectModel;
//using AndroidX.Emoji2.Text.FlatBuffer;
using Npgsql;
//using static Java.Text.Normalizer;
using Dapper;
namespace University;

//TODO:
//1. Make ListView function - i pass what i want to see
//2. Enable user to go back on ListView
//3. Enable user to go into ListView:
//  -if he goes into Year then show all possible Lectures, if he goes into lectures then show users
//  -if he goes into Lectures show years, then students
//  -if he goes into Students then show Name of those students, if he goes into Students then show Years, after Years lectures he went to

// 4. Search bar that will search for students/years/lectures and show scrolling list under search bar
// 5. Add button that will add new student/year/lecture
// 6. On click on student/year/lecture show details about it with option to edit it or delete it



public partial class MainPage : ContentPage
{

    public ObservableCollection<Student> Students { get; set; }
    public ObservableCollection<Year> Years { get; set; }
    public ObservableCollection<Lecture> Lectures { get; set; }
    public MainPage()
    {

        DatabaseHandler dbh = new DatabaseHandler();
        InitializeComponent();

    }
    private void SearchBarChanged(object sender, EventArgs e)
    {
        int i = 0;
    }
    private void DataSelectorChanged(object sender, EventArgs e)
    {
        SecondList.IsVisible = false;
        ThirdList.IsVisible = false;
        SecondColumn.Text = "";
        ThirdColumn.Text = "";
        var selectedIndex = DataSelector.SelectedIndex;
        switch (selectedIndex)
        {
            case 0:
                List<Student> students = DatabaseHandler.GetStudents();
                Students = new ObservableCollection<Student>(students);
                FirstList.ItemsSource = Students;
                FirstList.ItemTemplate = new DataTemplate(() =>
                {
                    var textCell = new TextCell();
                    textCell.SetBinding(TextCell.TextProperty, "nazwisko");
                    textCell.SetBinding(TextCell.DetailProperty, "imie");
                    return textCell;
                });
                break;
            case 1:
                List<Year> years = DatabaseHandler.GetYears();
                Years = new ObservableCollection<Year>(years);
                FirstList.ItemsSource = Years;
                FirstList.ItemTemplate = new DataTemplate(() =>
                {
                    var textCell = new TextCell();
                    textCell.SetBinding(TextCell.TextProperty, "rok");
                    return textCell;
                });
                break;
            case 2:
                List<Lecture> lectures = DatabaseHandler.GetLectures();
                Lectures = new ObservableCollection<Lecture>(lectures);
                FirstList.ItemsSource = Lectures;
                FirstList.ItemTemplate = new DataTemplate(() =>
                {
                    var textCell = new TextCell();
                    textCell.SetBinding(TextCell.TextProperty, "nazwa");
                    return textCell;
                });
                break;
        }
    }
    private void FirstListItemSelected(object sender, EventArgs e)
    {
        SecondList.IsVisible = false;
        ThirdList.IsVisible = false;
        SecondColumn.Text = "";
        ThirdColumn.Text = "";
        var selectedItem = DataSelector.SelectedItem as string;
        if(selectedItem == "Student")
        {
            Student selectedStudent = FirstList.SelectedItem as Student;
            //open window with details about student
        }
        else if (selectedItem == "Rok")
        {
            Year selectedYear = FirstList.SelectedItem as Year;
            List<Lecture> lectures = DatabaseHandler.GetLecturesByYear(selectedYear.rok);
            if (lectures.Count == 0)
            {
                Application.Current.MainPage.DisplayAlert("Alert", "Brak rekordów", "OK");
                return;
            }
            SecondList.ItemsSource = new ObservableCollection<Lecture>(lectures);
            SecondList.ItemTemplate = new DataTemplate(() =>
            {
                var textCell = new TextCell();
                textCell.SetBinding(TextCell.TextProperty, "nazwa");
                return textCell;
            });
            SecondColumn.Text = "Przedmioty";
            SecondList.IsVisible = true;
        }
        else if (selectedItem == "Przedmiot")
        {
            Lecture selectedLecture = FirstList.SelectedItem as Lecture;
            List<Year> years = DatabaseHandler.GetYearsByLecture(selectedLecture.nazwa);
            if (years.Count == 0)
            {
                Application.Current.MainPage.DisplayAlert("Alert", "Brak rekordów", "OK");
                return;
            }
            SecondList.ItemsSource = new ObservableCollection<Year>(years);
            SecondList.ItemTemplate = new DataTemplate(() =>
            {
                var textCell = new TextCell();
                textCell.SetBinding(TextCell.TextProperty, "rok");
                return textCell;
            });
            SecondColumn.Text = "Lata";
            SecondList.IsVisible = true;
        }
    }
    private void SecondListItemSelected(object sender, EventArgs e)
    {
        ThirdList.IsVisible=false;
        ThirdColumn.Text = "";
        var selectedItem = DataSelector.SelectedItem as string;
        if (selectedItem == "Rok")
        {
            Year selectedYear = FirstList.SelectedItem as Year;
            Lecture selectedLecture = SecondList.SelectedItem as Lecture;
            List<Student> students = DatabaseHandler.GetStudentsByYearAndLecture(selectedYear.rok, selectedLecture.nazwa);
            if (students.Count == 0)
            {
                Application.Current.MainPage.DisplayAlert("Alert", "Brak rekordów", "OK");
                return;
            }
            ThirdList.ItemsSource = new ObservableCollection<Student>(students);
            ThirdList.ItemTemplate = new DataTemplate(() =>
            {
                var textCell = new TextCell();
                textCell.SetBinding(TextCell.TextProperty, "nazwisko");
                textCell.SetBinding(TextCell.DetailProperty, "imie");
                return textCell;
            });
            ThirdColumn.Text = "Studenci";
            ThirdList.IsVisible = true;

        }
        else if (selectedItem == "Przedmiot")
        {
            Year selectedYear = SecondList.SelectedItem as Year;
            Lecture selectedLecture = FirstList.SelectedItem as Lecture;
            List<Student> students = DatabaseHandler.GetStudentsByYearAndLecture(selectedYear.rok, selectedLecture.nazwa);
            if (students.Count == 0)
            {
                Application.Current.MainPage.DisplayAlert("Alert", "Brak rekordów", "OK");
                return;
            }
            ThirdList.ItemsSource = new ObservableCollection<Student>(students);
            ThirdList.ItemTemplate = new DataTemplate(() =>
            {
                var textCell = new TextCell();
                textCell.SetBinding(TextCell.TextProperty, "nazwisko");
                textCell.SetBinding(TextCell.DetailProperty, "imie");
                return textCell;
            });
            ThirdColumn.Text = "Studenci";
            ThirdList.IsVisible = true;
        }
    }

    private void ThirdListItemSelected(Object sender, EventArgs e)
    {
        //open window
    }

    void StudentAdd_Clicked(System.Object sender, System.EventArgs e)
    {
    }
    void YearAdd_Clicked(System.Object sender, System.EventArgs e)
    {
    }
    void LectureAdd_Clicked(System.Object sender, System.EventArgs e)
    {
    }
}

public class Student
{
    public int id { get; set;  }
	public string imie { get; set; }
    public string nazwisko { get; set; }
    public DateTime data_urodzenia  { get; set; }
}

public class Year
{
    public int id { get; set; }
    public int rok { get; set; }
    public int id_studenta { get; set; }
}

public class Lecture
{
    public int id { get; set; }
    public string tag { get; set; }
    public string nazwa { get; set; }
    public int rok { get; set; }
    public int id_studenta { get; set; }
}


// DB:
// STUDENT: (KLUCZGLOWNY:{id}) {imie} {nazwisko} {data_urodzenia}
// YEAR: (KLUCZGLOWNY: {id}) {rok} {id_studenta}
// LECTURE: (KLUCZGLOWNY: {id}) {tag} {nazwa} {rok} {id_studenta}

public class DatabaseHandler
{
    private static NpgsqlConnection connection;
    public DatabaseHandler()
    {
        var cs = "Host=localhost;Port=5432;Username=myuser;database=ProjectDB";
        try
        {
            connection = new NpgsqlConnection(cs);
            connection.Open();
            CreateTables();
        }
        catch (Exception e)
        {
            Console.Write("BLAD BAZY DANYCH");
        }
    }
    private static void CreateTables(){
        string checkIfCreatedquery = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public';";
        bool checkifCreated = connection.ExecuteScalar<int?>(checkIfCreatedquery, new { })>0?true:false;
        if (checkifCreated)
        {
            return;
        }
        string CreateStudentQuery = "CREATE TABLE student(id SERIAL PRIMARY KEY, imie varchar(50), nazwisko varchar(50), data_urodzenia DATE)";
        string CreateYearQuery = "CREATE TABLE year(rok INT PRIMARY KEY, id_studenta INT REFERENCES student(id))";
        string CreateLectureQuery = "CREATE TABLE lecture(id SERIAL PRIMARY KEY, tag varchar(5), nazwa varchar(50), rok INT REFERENCES year(rok), id_studenta INT REFERENCES student(id))";
        connection.ExecuteScalar(CreateStudentQuery, new { });
        connection.ExecuteScalar(CreateYearQuery, new { });
        connection.ExecuteScalar(CreateLectureQuery, new { });
    }
    public static void AddStudent(string imie, string nazwisko, DateTime data_urodzenia)
    {
        string AddStudentQuery = "INSERT INTO student(imie, nazwisko, data_urodzenia) VALUES (@imie, @nazwisko, @data_urodzenia)";
        connection.ExecuteScalar(AddStudentQuery, new { imie = imie, nazwisko = nazwisko, data_urodzenia = data_urodzenia });
    }
    public static void AddYear(int rok, int id_studenta)
    {
        string AddYearQuery = "INSERT INTO year(rok, id_studenta) VALUES (@rok, @id_studenta)";
        connection.ExecuteScalar(AddYearQuery, new { rok = rok, id_studenta = id_studenta });
    }
    public static void AddLecture(string tag, string nazwa, int rok, int id_studenta)
    {
        string AddLectureQuery = "INSERT INTO lecture(tag, nazwa, rok, id_studenta) VALUES (@tag, @nazwa, @rok, @id_studenta)";
        connection.ExecuteScalar(AddLectureQuery, new { tag = tag, nazwa = nazwa, rok = rok, id_studenta = id_studenta });
    }
    public static void DeleteStudent(int id)
    {
        string DeleteStudentQuery = "DELETE FROM student WHERE id = @id";
        connection.ExecuteScalar(DeleteStudentQuery, new { id = id });
    }
    public static void DeleteYear(int id)
    {
        string DeleteYearQuery = "DELETE FROM year WHERE id = @id";
        connection.ExecuteScalar(DeleteYearQuery, new { id = id });
    }
    public static void DeleteLecture(int id)
    {
        string DeleteLectureQuery = "DELETE FROM lecture WHERE id = @id";
        connection.ExecuteScalar(DeleteLectureQuery, new { id = id });
    }
    public static void UpdateStudent(int id, string imie, string nazwisko, DateTime data_urodzenia)
    {
        string UpdateStudentQuery = "UPDATE student SET imie = @imie, nazwisko = @nazwisko, data_urodzenia = @data_urodzenia WHERE id = @id";
        connection.ExecuteScalar(UpdateStudentQuery, new { id = id, imie = imie, nazwisko = nazwisko, data_urodzenia = data_urodzenia });
    }
    public static void UpdateYear(int id, int rok, int id_studenta)
    {
        string UpdateYearQuery = "UPDATE year SET rok = @rok, id_studenta = @id_studenta WHERE id = @id";
        connection.ExecuteScalar(UpdateYearQuery, new { id = id, rok = rok, id_studenta = id_studenta });
    }
    public static void UpdateLecture(int id, string tag, string nazwa, int rok, int id_studenta)
    {
        string UpdateLectureQuery = "UPDATE lecture SET tag = @tag, nazwa = @nazwa, rok = @rok, id_studenta = @id_studenta WHERE id = @id";
        connection.ExecuteScalar(UpdateLectureQuery, new { id = id, tag = tag, nazwa = nazwa, rok = rok, id_studenta = id_studenta });
    }
    public static List<Student> GetStudents()    {
        string GetStudentsQuery = "SELECT * FROM student";
        return connection.Query<Student>(GetStudentsQuery, new { }).ToList();
    }
    public static List<Year> GetYears()    {
        string GetYearsQuery = "SELECT * FROM year";
        return connection.Query<Year>(GetYearsQuery, new { }).ToList();
    }
    public static List<Lecture> GetLectures()    {
        string GetLecturesQuery = "SELECT * FROM lecture";
        return connection.Query<Lecture>(GetLecturesQuery, new { }).ToList();
    }
    public static List<Student> GetStudentsByYear(int rok)
    {
        string GetStudentsByYearQuery = "SELECT * FROM student WHERE id IN (SELECT id_studenta FROM year WHERE rok = @rok)";
        return connection.Query<Student>(GetStudentsByYearQuery, new { rok = rok }).ToList();
    }
    public static List<Student> GetStudentsByLecture(string nazwa)
    {
        string GetStudentsByLectureQuery = "SELECT * FROM student WHERE id IN (SELECT id_studenta FROM lecture WHERE nazwa = @nazwa)";
        return connection.Query<Student>(GetStudentsByLectureQuery, new { nazwa = nazwa }).ToList();
    }
    public static List<Year> GetYearsByStudent(int id_studenta)
    {
        string GetYearsByStudentQuery = "SELECT * FROM year WHERE id_studenta = @id_studenta";
        return connection.Query<Year>(GetYearsByStudentQuery, new { id_studenta = id_studenta }).ToList();
    }
    public static List<Year> GetYearsByLecture(string nazwa)
    {
        string GetYearsByLectureQuery = "SELECT * FROM year WHERE rok IN (SELECT rok FROM lecture WHERE nazwa = @nazwa)";
        return connection.Query<Year>(GetYearsByLectureQuery, new { nazwa = nazwa }).ToList();
    }
    public static List<Lecture> GetLecturesByStudent(int id_studenta)
    {
        string GetLecturesByStudentQuery = "SELECT * FROM lecture WHERE id_studenta = @id_studenta";
        return connection.Query<Lecture>(GetLecturesByStudentQuery, new { id_studenta = id_studenta }).ToList();
    }
    public static List<Lecture> GetLecturesByYear(int rok)
    {
        string GetLecturesByYearQuery = "SELECT * FROM lecture WHERE rok = @rok";
        return connection.Query<Lecture>(GetLecturesByYearQuery, new { rok = rok }).ToList();
    }
    public static List<Student> GetStudentsByYearAndLecture(int rok, string nazwa)
    {
        string GetStudentsByYearAndLectureQuery = "SELECT * FROM student WHERE id IN (SELECT id_studenta FROM lecture WHERE rok = @rok AND nazwa = @nazwa)";
        return connection.Query<Student>(GetStudentsByYearAndLectureQuery, new { rok = rok, nazwa = nazwa }).ToList();
    }
    
}


