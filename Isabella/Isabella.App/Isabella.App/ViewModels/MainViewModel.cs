namespace Isabella.App.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// MainViewModel
    /// </summary>
    public class MainViewModel
    {
        /*Implementando el patrón Singleton*/
        private static MainViewModel instance;

        public MainViewModel()
        {
            instance = this;
        }

        public MainViewModel GetInstance()
        {
            if(instance == null)
            instance = new MainViewModel();
            return instance;
        }

        /*Relación con las otras ViewModel*/
        public LoginViewModel Login { get; set; }
    }
}
