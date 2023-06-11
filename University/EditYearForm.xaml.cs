namespace University;

public partial class EditYearForm : ContentPage
{
    private Year year;
	public EditYearForm()
	{
		InitializeComponent();
	}
    public void getData(Year rok)
    {
        this.year = rok;
        RokEntry.Text = year.rok.ToString();
    }
    private async void PotwierdzButton_Clicked(object sender, EventArgs e)
    {
        /*var response = await DisplayAlert("Potwierdzenie", "Czy na pewno chcesz edytować?", "Tak", "Anuluj");

        if (response)
        {*/
            var students = DatabaseHandler.GetStudentsByYear(year.id);
            var rok = Convert.ToInt32(RokEntry.Text);
            DatabaseHandler.EditYear(year.rok, rok);
            Navigation.PopAsync();
        //}
    }
}
