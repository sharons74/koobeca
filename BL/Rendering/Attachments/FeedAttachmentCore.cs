using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Rendering {
    public class FeedAttachmentCore : FeedAttachment {
        public string title;
        public string body;
        public string attachment_type = "core_link";
        public uint attachment_id;
        public string uri;
        public FeedImage image_main;
        public uint mode;


        public FeedAttachmentCore(ActivityAttachment attachment,ulong actionId) {
            
            var coreLink = CoreLinks.GetById(attachment.id);
            title = coreLink.title;
            body = HttpUtility.HtmlDecode(coreLink.description);
            //uri = $"https://beta.koobeca.com/core/link/index/id/{coreLink.link_id}/key/{Md5.CalculateMd5Hash(coreLink.link_id + coreLink.uri)}/action_id/{actionId}";
            uri = $"https://beta.koobeca.com/view.php?action_id={actionId}";
            image_main = coreLink.photo_id > 0 ? new FeedImage(coreLink.photo_id) : null;
            if (image_main == null) { // && !string.IsNullOrEmpty(coreLink.@params)){
                var iframely = coreLink.GetIFramely();
                if (iframely != null)
                    try {
                        image_main = new FeedImage(iframely);
                    }
                    catch {
                        Logger.Instance.Error($"Bad iframley in corelink {coreLink.link_id} : {coreLink.@params}");
                    }
            }

            attachment_id = attachment.id;
            mode = (uint) (attachment.mode ? 1 : 0);
        }

        public override bool HasPhoto => image_main != null;

        public override List<string> TagList {
            get {
                if (!string.IsNullOrEmpty(title))
                {
                    var list = HashTagFinder.FindTags(title);
                    list.AddRange(HashTagFinder.FindTags(body));
                    return list.GroupBy(test => test).Select(grp => grp.First()).ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
        }
    }
}