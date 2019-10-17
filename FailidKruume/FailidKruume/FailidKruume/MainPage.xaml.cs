using FailidKruume;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin;
using Plugin.TextToSpeech;

namespace FileApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await UpdateFileList();
        }
        // сохранение текста в файл
        async void Save(object sender, EventArgs args)
        {
            string filename = fileNameEntry.Text;
            if (String.IsNullOrEmpty(filename)) return;
            // если файл существует
            if (await DependencyService.Get<IFileWorker>().ExistsAsync(filename))
            {
                // запрашиваем разрешение на перезапись
                bool isRewrited = await DisplayAlert("Подверждение", "Файл уже существует, перезаписать его?", "Да", "Нет");
                if (isRewrited == false) return;
            }
            // перезаписываем файл
            await DependencyService.Get<IFileWorker>().SaveTextAsync(fileNameEntry.Text, textEditor.Text);
            // обновляем список файлов
            await UpdateFileList();
        }
        async void FileSelect(object sender, SelectedItemChangedEventArgs args)
        {
            if (args.SelectedItem == null) return;
            // получаем выделенный элемент
            string filename = (string)args.SelectedItem;
            // загружем текст в текстовое поле
            textEditor.Text = await DependencyService.Get<IFileWorker>().LoadTextAsync((string)args.SelectedItem);
            // устанавливаем название файла
            fileNameEntry.Text = filename;
            // снимаем выделение
            filesList.SelectedItem = null;

        }
        async void Delete(object sender, EventArgs args)
        {
            // получаем имя файла
            string filename = (string)((MenuItem)sender).BindingContext;
            // удаляем файл из списка
            await DependencyService.Get<IFileWorker>().DeleteAsync(filename);
            // обновляем список файлов
            await UpdateFileList();
        }
        // обновление списка файлов
        async Task UpdateFileList()
        {
            // получаем все файлы
            filesList.ItemsSource = await DependencyService.Get<IFileWorker>().GetFilesAsync();
            // снимаем выделение
            filesList.SelectedItem = null;
        }

        private void Voice(object sender, EventArgs e)
        {
            var Text = textEditor.Text;
            CrossTextToSpeech.Current.Speak(Text);

        }
    }
}