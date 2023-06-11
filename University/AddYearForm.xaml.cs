using System.Collections.Generic;
using System.Formats.Tar;

namespace University;

public partial class AddYearForm : ContentPage
{
    List<Student> students;
    public AddYearForm()
	{
        InitializeComponent();
    }
    public void getStudentList(List<Student> studentList)
    {
        students = studentList;

        foreach (var student in students)
        {
            studentPicker.Items.Add($"{student.imie} {student.nazwisko}");
        }
    }

    private void ConfirmButton_Clicked(object sender, EventArgs e)
    {
        string selectedStudentName = studentPicker.SelectedItem.ToString();
        int year = 0;
        try
        {
            year = Convert.ToInt32(yearEntry.Text.ToString());
        }
        catch(Exception f)
        {
            DisplayAlert("BŁĄD", "Niepoprawny rok!", "OK");
        }
        Student selectedStudent = students.Find(student =>
            $"{student.imie} {student.nazwisko}" == selectedStudentName);

        if (selectedStudent != null)
        {
            DatabaseHandler.AddYear(year, selectedStudent.id);
            Navigation.PopAsync();
        }
        else
        {
            DisplayAlert("Błąd", "Nie wybrano poprawnego studenta", "OK");
        }
    }
}

