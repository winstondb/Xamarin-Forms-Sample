using DiaApp.Models;
using DiaApp.Services;
using DiaApp.Views;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DiaApp.ViewModels
{
    public class ChangePasswordViewModel : INotifyPropertyChanged
    {
        #region Declare
        private NavigationService _NavigationService;
        private DialogService _DialogService;
        private APIService _APIService;
        private DataService _DataService;
        private NetService _NetService;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ICommand HomeCommand { get { return new RelayCommand(Home); } }
        public ICommand ChangePasswordCommand { get { return new RelayCommand(ChangePasswordSave); } }
        #endregion

        #region Construtor
        public ChangePasswordViewModel()
        {
            _NavigationService = new NavigationService();
            _DialogService = new DialogService();
            _APIService = new APIService();
            _DataService = new DataService();
            _NetService = new NetService();
        }
        #endregion

        #region Properties
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string RepeatPassword { get; set; }

        private bool _IsRunning;
        public bool IsRunning
        {
            get
            {
                return _IsRunning;
            }
            set
            {
                _IsRunning = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Command
        private void Home()
        {
            App.Current.MainPage = new MasterPage();
        }

        private async void ChangePasswordSave()
        {
            var _User = _DataService.Get<User>(false).FirstOrDefault();

            if (string.IsNullOrEmpty(Password))
            {
                await _DialogService.ShowMessage("Aviso", "Digite a SENHA ATUAL");
                return;
            }

            // Checar a SENHA ATUAL
            if (Password != _User.Password)
            {
                await _DialogService.ShowMessage("Aviso", "SENHA ATUAL incorreta!");
                return;
            }

            if (string.IsNullOrEmpty(NewPassword))
            {
                await _DialogService.ShowMessage("Aviso", "Digite a NOVA SENHA");
                return;
            }

            if (string.IsNullOrEmpty(RepeatPassword))
            {
                await _DialogService.ShowMessage("Aviso", "Digite a NOVA SENHA novamente");
                return;
            }

            if (NewPassword != RepeatPassword)
            {
                await _DialogService.ShowMessage("Aviso", "NOVA SENHA está divergente! Digite novamente");
                return;
            }

            var answer = await _DialogService.ShowConfirm("Aguardando resposta...", "Deseja salvar?");
            if (!answer)
            {
                return;
            }

            IsRunning = true;

            if (_NetService.IsConnected())
            {
                if (_User != null)
                {
                    _User.Password = NewPassword;
                    _DataService.Update(_User);

                    IsRunning = true;

                    string Message = "";
                    if (!_APIService.UpdatePassword(_User, ref Message))
                    {
                        await _DialogService.ShowMessage("Aviso!", Message);
                        goto End;
                    }

                    await _DialogService.ShowMessage("Aviso!", "Registro sincronizado");

                    End:;
                    IsRunning = false;
                }
            }

            await _NavigationService.Back();

            IsRunning = false;
        }
        #endregion
    }
}
