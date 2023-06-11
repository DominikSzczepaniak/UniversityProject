namespace University;

public partial class EditLectureForm : ContentPage
{
    private Lecture lecture;
    public EditLectureForm()
	{
        InitializeComponent();
    }
    public void getData(Lecture lect)
    {
        this.lecture = lect;
        var years = DatabaseHandler.GetYears();
        foreach (var year in years)
        {
            RokPicker.Items.Add(year.rok.ToString());
        }
        RokPicker.SelectedItem = lecture.rok.ToString();
        TagEntry.Text = lect.tag.ToString();
        NazwaEntry.Text = lect.nazwa.ToString();
    }
    private async void PotwierdzButton_Clicked(object sender, EventArgs e)
    {
        /*var response = await DisplayAlert("Potwierdzenie", "Czy na pewno chcesz edytować?", "Tak", "Anuluj");

        if (response)
        {*/
            var students = DatabaseHandler.GetStudentsByLecture(lecture.nazwa);
            var rok = RokPicker.SelectedItem as Year;
            DatabaseHandler.EditLecture(lecture.nazwa, NazwaEntry.Text, lecture.tag, TagEntry.Text, lecture.rok, rok.id);
            Navigation.PopAsync();
        //}
    }

}
