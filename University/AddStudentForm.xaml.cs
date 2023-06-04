namespace University;

public partial class AddStudentForm : ContentPage
{
	public AddStudentForm()
	{
		InitializeComponent();
	}
    private void ConfirmButton_Clicked(object sender, EventArgs e)
    {
        string firstName = firstNameEntry.Text;
        string lastName = lastNameEntry.Text;
        DateTime birthDate = birthDatePicker.Date;
        DatabaseHandler.AddStudent(firstName, lastName, birthDate);
    }
}
