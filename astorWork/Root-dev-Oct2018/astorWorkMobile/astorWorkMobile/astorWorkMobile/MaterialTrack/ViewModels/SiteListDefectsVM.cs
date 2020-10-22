using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class SiteListDefectsVM : MasterVM
    {
        public int StageAuditId { get; set; }

        public async Task GetQCCaseDetails()
        {
            if (StageAuditId > 0)
            {
                var result = await ApiClient.Instance.MTGetQCCases(StageAuditId);
                if (result.data != null)
                {
                    var listCases = result.data as List<QCCase>;
                    OpenCase = listCases?.Find(c => c.IsOpen);

                    if (OpenCase == null)
                    {
                        ErrorMessage = "Cannot find any open QC case for this material";
                        /*
                        var strListCases = JsonConvert.SerializeObject(listCases);
                        ErrorMessage = $"Material <{Material.id}> has following QC cases: {strListCases}";
                        */
                    }
                }
                else
                    ErrorMessage = result.message;
            }
            else
                ErrorMessage = "No material specified";
        }

        /*
        private ObservableCollection<QCCase> _listQCCase;
        public ObservableCollection<QCCase> ListQCCase { get
            {
                return _listQCCase;
            }
            set
            {
                _listQCCase = value;
                OnPropertyChanged("ListQCCase");
            }
        }
        */

        private QCCase _openCase;
        public QCCase OpenCase
        {
            get
            {
                return _openCase;
            }
            set
            {
                _openCase = value;
                RefreshCase();
                OnPropertyChanged("OpenCase");
            }
        }

        public void RefreshCase()
        {
            if (_openCase != null)
            {
                _openCase.RefreshDefects();
            }
        }

        public override void Reset()
        {
            base.Reset();
            OpenCase = null;
        }
    }
}
