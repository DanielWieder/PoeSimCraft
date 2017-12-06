using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainWindowModule.Toolbar
{
    public class ToolbarViewModel
    {
        //    DelegateCommand _saveCommand;
        //    public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(OnRequestSave));

        //    DelegateCommand _openCommand;
        //  public ICommand OpenCommand => _openCommand ?? (_openCommand = new DelegateCommand(OnRequestOpen));

        //private void OnRequestOpen()
        //{

        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    openFileDialog.Multiselect = false;
        //    openFileDialog.Filter = "Json files (*.json)|*.json|All files (*.*)|*.*";
        //    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //    if (openFileDialog.ShowDialog() == DialogResult.OK)
        //    {
        //        StreamReader reader = new StreamReader(openFileDialog.FileName);
        //        var text = reader.ReadToEnd();
        //        var loader = new LoadSavedData();

        //        var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<SimulationJson>(text);
        //        var SimData = loader.Execute(jsonObject);

        //        _configRepository.SetItemConfig(SimData.Config);
        //    }
        //}

        //private void OnRequestSave()
        //{
        //    SimulationJson combinedObj = new SimulationJson();
        //    combinedObj.ItemConfig = _configRepository.GetItemConfig();

        //    if (combinedObj.ItemConfig.IsValid)
        //    {
        //        var combinedJson = Newtonsoft.Json.JsonConvert.SerializeObject(combinedObj);

        //        SaveText("Save Crafting Configuration", combinedJson);
        //    }
        //}

        //private static void SaveText(string title, string text)
        //{
        //    SaveFileDialog saveFileDialog = new SaveFileDialog();
        //    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //    saveFileDialog.Title = title;
        //    saveFileDialog.CheckPathExists = true;
        //    saveFileDialog.DefaultExt = "json";
        //    saveFileDialog.Filter = "Json files (*.json)|*.json|All files (*.*)|*.*";
        //    saveFileDialog.FilterIndex = 1;
        //    saveFileDialog.RestoreDirectory = true;

        //    if (saveFileDialog.ShowDialog() == DialogResult.OK)
        //    {
        //        using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
        //            sw.Write(text);
        //    }
        //}
    }
}
