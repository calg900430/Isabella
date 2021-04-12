namespace Isabella.App.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Input;
    using Xamarin.Forms;

    using GalaSoft.MvvmLight.Command;

    using Views;

    /// <summary>
    /// LoginViewModel
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        /// <summary>
        /// Botón de Login.
        /// </summary>
        public ICommand LoginCommand => new RelayCommand(Login);

        /// <summary>
        /// Correo.
        /// </summary>
        private string _email;
        public string Email { get => this._email; set => this.SetProperty(ref this._email, value); }

        /// <summary>
        /// Password.
        /// </summary>
        private string _password;
        public string Password { get => this._password; set => this.SetProperty(ref this._password, value); }

        /// <summary>
        /// IsEnabled.
        /// </summary>
        private bool _isenabled;
        public bool IsEnabled { get => this._isenabled; set => this.SetProperty(ref this._isenabled, value); }

        /// <summary>
        /// Constructor
        /// </summary>
        public LoginViewModel()
        {
            Title = "Login";
            this.IsEnabled = true;
            this.IsBusy = false;
        }

        /// <summary>
        /// Login
        /// </summary>
        private async void Login()
        {
            //Verifica que el correo no este nulo ni vacio.
            if (string.IsNullOrEmpty(Email))
            {
                await Application.Current.MainPage
                .DisplayAlert("Error", "You must enter an email.", "Ok");
                return;
            }
            //Verifica que el password no este nulo ni vacio.
            if (string.IsNullOrEmpty(Password))
            {
                await Application.Current.MainPage
                .DisplayAlert("Error", "You must enter a password.", "Ok");
                return;
            }
            IsBusy = true;  //Activa el ActivityIndicator
            IsEnabled = false; //Pone los botones en false
            var datalogin = userMockService //Inicia el proceso de Login
            .LoginMock(Email, Password);
            IsBusy = false; //Desactiva el ActivityIndicator
            IsEnabled = true;  //Pone los botones en true
            //Pasa a la MasterPage pero la coloca dentro de una NavigationPage.
            Application.Current.MainPage = new NavigationPage(new MasterPage());
        }
    }
}
