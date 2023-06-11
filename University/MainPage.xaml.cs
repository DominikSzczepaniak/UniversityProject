using System.Collections.ObjectModel;
using Npgsql;
using Dapper;
namespace University;

//TODO:
// 1. Check for duplicates in main lists (this file), and repair showing next lists and it's done.

public partial class MainPage : ContentPage
{

    public ObservableCollection<Student> Students { get; set; }
    public ObservableCollection<Year> Years { get; set; }
    public ObservableCollection<Lecture> Lectures { get; set; }
    public bool IsChecked1 { get; set; }
    public bool IsChecked2 { get; set; }
    public MainPage()
    {

        DatabaseHandler dbh = new DatabaseHandler();
        InitializeComponent();

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
                Years = new ObservableCollection<Year>(years.DistinctBy(p=>p.rok));
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
                Lectures = new ObservableCollection<Lecture>(lectures.DistinctBy(p=>p.nazwa));
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
    private async void EditLect(Lecture selectedLecture)
    {
        var a1 = new EditLectureForm();
        a1.getData(selectedLecture);
        await Navigation.PushAsync(a1);

    }
    private async void EditYear(Year selectedYear)
    {
        var a1 = new EditYearForm();
        a1.getData(selectedYear);
        await Navigation.PushAsync(a1);
        
    }
    private async void openStudentInfo(Student student)
    {
        var a1 = new StudentInfo();
        a1.getData(student);
        await Navigation.PushAsync(a1);
    }
    private void FirstListItemSelected(object sender, EventArgs e)
    {
        SecondList.IsVisible = false;
        ThirdList.IsVisible = false;
        SecondColumn.Text = "";
        ThirdColumn.Text = "";
        var selectedItem = DataSelector.SelectedItem as string;
        if (EditRadioButton.IsChecked)
        {
            if (selectedItem == "Student")
            {
                openStudentInfo(FirstList.SelectedItem as Student);
            }
            else if (selectedItem == "Przedmiot")
            {
                EditLect(FirstList.SelectedItem as Lecture);
            }
            else if (selectedItem == "Rok")
            {
                EditYear(FirstList.SelectedItem as Year);
            }
            return;
        }

        if (DeleteRadioButton.IsChecked)
        {
            if (selectedItem == "Przedmiot")
            {
                var lect = FirstList.SelectedItem as Lecture;
                DatabaseHandler.DeleteLectureByNazwa(lect.nazwa);
            }
            else if (selectedItem == "Rok")
            {
                var year = FirstList.SelectedItem as Year;
                DatabaseHandler.DeleteYearByRok(year.rok);
            }
            else if(selectedItem == "Student")
            {
                var student = FirstList.SelectedItem as Student;
                DatabaseHandler.DeleteStudent(student.id);
            }
            DataSelectorChanged(sender, e);
            return;
        }
        if (selectedItem == "Student")
        {
            Student selectedStudent = FirstList.SelectedItem as Student;
            openStudentInfo(selectedStudent);
        }
        else if (selectedItem == "Rok")
        {
            Year selectedYear = FirstList.SelectedItem as Year;
            List<Lecture> lectures = DatabaseHandler.GetLecturesByRok(selectedYear.rok);
            
            if (lectures.Count == 0)
            {
                Application.Current.MainPage.DisplayAlert("Alert", "Brak rekordów", "OK");
                return;
            }
            SecondList.ItemsSource = new ObservableCollection<Lecture>(lectures.DistinctBy(p=>p.nazwa));
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
            SecondList.ItemsSource = new ObservableCollection<Year>(years.DistinctBy(p=>p.rok));
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
        if (EditRadioButton.IsChecked)
        {
            if (selectedItem == "Przedmiot")
            {
                EditYear(SecondList.SelectedItem as Year);
            }
            else if (selectedItem == "Rok")
            {
                EditLect(SecondList.SelectedItem as Lecture);
            }
            return;
        }
        if (DeleteRadioButton.IsChecked)
        {
            if (selectedItem == "Przedmiot")
            {
                //usun rok
                var year = SecondList.SelectedItem as Year;
                DatabaseHandler.DeleteYearByRok(year.rok);
            }
            else if (selectedItem == "Rok")
            {
                var lect = SecondList.SelectedItem as Lecture;
                DatabaseHandler.DeleteLectureByNazwa(lect.nazwa);
            }
            FirstListItemSelected(sender, e);
            return;
        }
        if (selectedItem == "Rok")
        {
            Year selectedYear = FirstList.SelectedItem as Year;
            Lecture selectedLecture = SecondList.SelectedItem as Lecture;
            List<Student> students = DatabaseHandler.GetStudentsByNazwaAndRok(selectedLecture.nazwa, selectedYear.rok);
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
            List<Student> students = DatabaseHandler.GetStudentsByNazwaAndRok(selectedLecture.nazwa, selectedYear.rok);
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
        openStudentInfo(ThirdList.SelectedItem as Student);
    }

    async void StudentAdd_Clicked(System.Object sender, System.EventArgs e)
    {
        var a1 = new AddStudentForm();
        await Navigation.PushAsync(a1);
    }
    async void YearAdd_Clicked(System.Object sender, System.EventArgs e)
    {
        var a1 = new AddYearForm();
        a1.getStudentList(DatabaseHandler.GetStudents());
        await Navigation.PushAsync(a1);
    }
    async void LectureAdd_Clicked(System.Object sender, System.EventArgs e)
    {
        var a1 = new AddLectureForm();
        a1.getData(DatabaseHandler.GetStudents(), DatabaseHandler.GetYears());
        await Navigation.PushAsync(a1);
    }
    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value && sender == EditRadioButton)
        {
            DeleteRadioButton.IsChecked = false;
        }
        else if (e.Value && sender == DeleteRadioButton)
        {
            EditRadioButton.IsChecked = false;
        }
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
    public int id_roku { get; set; }
    public int id_studenta { get; set; }
    public int rok {get; set;}
    public void getRok()
    {
        rok = DatabaseHandler.GetRokByRokId(id_roku);
    }
}


