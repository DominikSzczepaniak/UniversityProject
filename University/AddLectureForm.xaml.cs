using System.Formats.Tar;

namespace University;

public partial class AddLectureForm : ContentPage
{
	List<Student> studentList;
	List<Year> yearList;
	public AddLectureForm()
	{
		InitializeComponent();
	}
	public void getData(List<Student> students, List<Year> years)
	{
		studentList = students;
		yearList = years;
        foreach (var student in students)
        {
            studentPicker.Items.Add($"{student.imie} {student.nazwisko}");
        }
        foreach (var rok in years)
        {
            yearPicker.Items.Add($"{rok.rok}");
        }
    }
    private void ConfirmButton_Clicked(object sender, EventArgs e)
    {
        string selectedStudentName = studentPicker.SelectedItem.ToString();
        string nazwa = NameEntry.Text;
        string TAG = TagEntry.Text;
        Student selectedStudent = studentList.Find(student =>
            $"{student.imie} {student.nazwisko}" == selectedStudentName);
        Year selectedYear = yearList.Find(rok => $"{rok.rok}" == yearPicker.SelectedItem.ToString());

        if (selectedStudent != null)
        {
            DatabaseHandler.AddLecture(TAG, nazwa, selectedYear.id, selectedStudent.id);
            Navigation.PopAsync();
        }
        else
        {
            DisplayAlert("Błąd", "Nie wybrano poprawnego studenta", "OK");
        }
    }
}
