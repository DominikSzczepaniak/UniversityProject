namespace University;

public partial class AddYearToStudentForm : ContentPage
{
	private Student currentStudent;
	public AddYearToStudentForm()
	{
		InitializeComponent();
	}
	public void getData(Student student)
	{
		currentStudent = student;
		StudentName.Text = student.imie + " " + student.nazwisko;
		var years = DatabaseHandler.GetYears();
		foreach(var year in years)
		{
			RokPicker.Items.Add(year.rok.ToString());
		}
	}
	private async void PotwierdzButton_Clicked(object sender, EventArgs e)
	{
        var response = await DisplayAlert("Potwierdzenie", "Czy na pewno chcesz edytować?", "Tak", "Anuluj");
		if (response)
		{
			if (!DatabaseHandler.checkIfStudentHasThisYear(currentStudent.id, Convert.ToInt32(RokPicker.SelectedItem.ToString())))
			{
				DatabaseHandler.AddRokForStudent(currentStudent.id, Convert.ToInt32(RokPicker.SelectedItem));
				
			}
            Navigation.PopAsync();
        }
		
    }
}
