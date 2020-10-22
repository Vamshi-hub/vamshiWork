using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.JobTrack.Pages.Cells;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace astorWorkMobile.JobTrack.Helpers
{
    class ChatTemplateSelector : DataTemplateSelector
    {
        DataTemplate incomingDataTemplate;
        DataTemplate outgoingDataTemplate;
        DataTemplate systemDataTemplate;

        private string _userName;

        public ChatTemplateSelector()
        {
            incomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            outgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));
            systemDataTemplate = new DataTemplate(typeof(SystemViewCell));

            if (Application.Current.Properties.ContainsKey("user_name"))
                _userName = Application.Current.Properties["user_name"] as string;
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var messageVm = item as ChatMessage;
            if (messageVm == null)
                return null;
            if (messageVm.IsSystem)
                return systemDataTemplate;
            else
                return (messageVm.UserName.ToLower() == _userName.ToLower()) ? outgoingDataTemplate : incomingDataTemplate;
            throw new NotImplementedException();
        }
    }
}
