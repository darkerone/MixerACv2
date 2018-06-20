using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace MixerACv2.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Properties
        
        private ObservableCollection<FileViewModel> _files = new ObservableCollection<FileViewModel>();
        public ObservableCollection<FileViewModel> Files { get { return _files; } }

        private FileViewModel _selectedFile;
        public FileViewModel SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public MainViewModel()
        {
        }

        #region Commands

        public ICommand AddFiles
        {
            get {
                return new DelegateCommand((param) => {

                    using (var fbd = new OpenFileDialog())
                    {
                        fbd.Multiselect = true;

                        DialogResult result = fbd.ShowDialog();

                        if (result == DialogResult.OK && fbd.FileNames != null && fbd.FileNames.Any())
                        {
                            foreach(string filePath in fbd.FileNames)
                            {
                                Files.Add(new FileViewModel(filePath));
                            }
                        }
                    }

                });
            }
        }

        public ICommand ClearFilesList
        {
            get
            {
                return new DelegateCommand((param) => {
                    Files.Clear();
                });
            }
        }

        public ICommand RemoveSelectedFile
        {
            get
            {
                return new DelegateCommand((param) => {
                    if(SelectedFile != null)
                    {
                        Files.Remove(SelectedFile);
                    }
                    
                    //SelectedFile = Files.FirstOrDefault();
                });
            }
        }

        public ICommand RenameFiles
        {
            get
            {
                return new DelegateCommand((param) => {
                    if (Files.Any())
                    {
                        // Détermine le nombre de chiffre nécessaires (1, 02, 003,...)
                        int numberOfDigit = Files.Count.ToString().Length;

                        // Récupère une liste aléatoire de nombres entre 0 et le nombre de fichiers
                        List<int> randomNumberList = this.GetRandomNumberList(0, Files.Count);

                        // Change le nouveau nom de chaque fichier en le préfixant avec un nombre de la liste aléatoire
                        for (int cpt = 0; cpt < Files.Count; cpt++)
                        {
                            Files[cpt].NewName = randomNumberList[cpt].ToString("D" + numberOfDigit) + "_" + Files[cpt].OldName;
                        }
                    }
                });
            }
        }

        public ICommand UnrenameFiles
        {
            get
            {
                return new DelegateCommand((param) => {
                    if (Files.Any())
                    {
                        foreach(FileViewModel fileVM in Files)
                        {
                            // Découpe la chaine selon les "_"
                            string[] splittedNameArray = fileVM.OldName.Split('_');
                            
                            List<string> splittedNameList = new List<string>(splittedNameArray);
                            // Si le premier élément est bien un nombre
                            int index;
                            if (Int32.TryParse(splittedNameList.First(), out index))
                            {
                                // Supprime le premier élément
                                splittedNameList.RemoveAt(0);
                                // Reconstitue la chaine sans le premier élément
                                fileVM.NewName = string.Join("_", splittedNameList);
                            }
                        }
                    }
                });
            }
        }

        public ICommand SaveFiles
        {
            get
            {
                return new DelegateCommand((param) => {
                    if (Files.Any())
                    {
                        foreach(FileViewModel fileVM in Files)
                        {
                            string filePathName = fileVM.FolderPath + "\\" + fileVM.NewName;
                            System.IO.File.Move(fileVM.FolderPath + "\\" + fileVM.OldName, filePathName);
                            fileVM.OldName = fileVM.NewName;
                        }
                    }
                });
            }
        }

        #endregion region

        #region Private methods

        /// <summary>
        /// Récupération d'une liste aléatoire de nombres entre min et max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private List<int> GetRandomNumberList(int min, int max)
        {
            if(max < min)
            {
                int tmp = max;
                max = min;
                min = tmp;
            }

            List<int> numberList = new List<int>();

            Random rnd = new Random();
            while(numberList.Count != max - min)
            {
                int number = rnd.Next(min, max);

                if (!numberList.Contains(number))
                {
                    numberList.Add(number);
                }
            }

            return numberList;
        }

        #endregion region
    }
}
