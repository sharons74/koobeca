using System;
using System.Linq;

namespace KoobecaFeedController.BL.Handlers
{
    internal class PromptCommand
    {
        private Activity activity;

        public PromptCommand(Activity activity)
        {
            this.activity = activity;
        }

        public void Execute(string cmd)
        {
            try
            {
                _Execute(cmd);
            }
            catch(Exception e)
            {
                activity.RawActivity.BodyStr = e.ToString();
                //return true;
            }
        }

        private void _Execute(string cmd) {
            cmd = cmd.Replace("kb:", "");
            var parts = cmd.Split(' ').Where(s => s != " ").ToList();
            //if(parts.Count > 0)
        }
    }
}