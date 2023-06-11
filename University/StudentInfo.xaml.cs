namespace University;

public partial class StudentInfo : TabbedPage
{
    private Student currentStudent;
    public StudentInfo()
	{
		InitializeComponent();
	}
    private void CreateLecturesTab()
    {
        var lectures = DatabaseHandler.GetLecturesByStudent(currentStudent.id);
        lecturesListView.ItemsSource = lectures;
    }
    private void CreateYearTab()
    {
        var years = DatabaseHandler.GetYearsByStudent(currentStudent.id);
        yearListView.ItemsSource = years;
    }
    private void YearRefresh(object sender, EventArgs e)
    {
        CreateYearTab();
    }
    private void LectureRefresh(object sender, EventArgs e)
    {
        CreateLecturesTab();
    }
    private void DeleteLectureButton_Clicked(object sender, System.EventArgs e)
    {
        var lecture = (sender as Button).BindingContext as Lecture;
        DatabaseHandler.DeleteLectureForStudent(lecture.id, currentStudent.id);
        CreateLecturesTab();
    }
    private void DeleteYearButton_Clicked(object sender, System.EventArgs e)
    {
        var year = (sender as Button).BindingContext as Year;
        DatabaseHandler.DeleteYearForStudent(year.id, currentStudent.id);
        CreateYearTab();
    }
    private async void AddLectureButton(Object sender, EventArgs e)
    {
        var a2 = new AddLectureToStudentForm();
        a2.getData(currentStudent);
        await Navigation.PushAsync(a2);
    }
    private async void AddYearButton(Object sender, EventArgs e)
    {
        var a2 = new AddYearToStudentForm();
        a2.getData(currentStudent);
        await Navigation.PushAsync(a2);
    }
    public void getData(Student student)
	{
		currentStudent = student;
        firstNameEntry.Text = currentStudent.imie;
        lastNameEntry.Text = currentStudent.nazwisko;
        birthdatePicker.Date = currentStudent.data_urodzenia;
        CreateLecturesTab();
    }
    private void FirstNameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        DatabaseHandler.EditStudent(currentStudent.id, currentStudent.imie, firstNameEntry.Text, currentStudent.nazwisko, currentStudent.nazwisko, currentStudent.data_urodzenia, currentStudent.data_urodzenia);
        currentStudent.imie = firstNameEntry.Text;
    }

    private void LastNameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        DatabaseHandler.EditStudent(currentStudent.id, currentStudent.imie, currentStudent.imie, currentStudent.nazwisko, lastNameEntry.Text, currentStudent.data_urodzenia, currentStudent.data_urodzenia);
        currentStudent.nazwisko = lastNameEntry.Text;
    }

    private void BirthdatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        DatabaseHandler.EditStudent(currentStudent.id, currentStudent.imie, currentStudent.imie, currentStudent.nazwisko, currentStudent.nazwisko, currentStudent.data_urodzenia, e.NewDate);
        currentStudent.data_urodzenia = e.NewDate;
    }
}
