namespace University;

public partial class AddLectureToStudentForm : ContentPage
{
    private Student currentStudent;
	public AddLectureToStudentForm()
	{
		InitializeComponent();
	}
    public void getData(Student student)
    {
        currentStudent = student;
        StudentName.Text = student.imie + " " + student.nazwisko;
        var lectures = DatabaseHandler.GetLectures();
        foreach (var lecture in lectures)
        {
            NazwaPicker.Items.Add(lecture.nazwa);
        }
        var years = DatabaseHandler.GetYears();
        foreach(var year in years)
        {
            if (RokPicker.Items.Contains(year.rok.ToString()))
            {
                continue;
            }

            RokPicker.Items.Add(year.rok.ToString());
        }
    }
    private async void PotwierdzButton_Clicked(object sender, EventArgs e)
    {
        var response = await DisplayAlert("Potwierdzenie", "Czy na pewno chcesz edytować?", "Tak", "Anuluj");
        if (response)
        {
            var rok = Convert.ToInt32(RokPicker.SelectedItem.ToString());
            var id_roku = DatabaseHandler.GetIdRokuByRok(rok);
            if (!DatabaseHandler.checkIfStudentHasThisLectureInYear(currentStudent.id, NazwaPicker.SelectedItem.ToString(), rok))
            {
                DatabaseHandler.AddLectureForStudentInYear(currentStudent.id, NazwaPicker.SelectedItem.ToString(), id_roku[0].id);
                
            }
            Navigation.PopAsync();
        }

    }
}
