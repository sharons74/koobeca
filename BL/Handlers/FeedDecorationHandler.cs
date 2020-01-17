using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.Handlers
{
    internal class FeedDecorationHandler : ActivityHandler
    {
        public FeedDecorationHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            Response = @"{ 
           ""feed_docoration_setting"":{ 
              ""char_length"":""100"",
              ""font_size"":""15"",
              ""font_color"":""#000"",
              ""font_style"":""normal"",
              ""banner_feed_length"":""100"",
              ""banner_count"":""10"",
              ""banner_order"":""random""
           },
           ""word_styling"":[
              { 
                 ""word_id"":3,
                 ""title"":""Happy New Year"",
                 ""color"":""#4f39e3"",
                 ""background_color"":""#FFFFFF"",
                 ""style"":""normal"",
                 ""params"":{ 
                    ""animation"":""background-happy-new-year"",
                    ""bg_enabled"":""0""
                 }
        },
              { 
                 ""word_id"":4,
                 ""title"":""Happy Birthday"",
                 ""color"":""#09961e"",
                 ""background_color"":""#ffffff"",
                 ""style"":""normal"",
                 ""params"":{ 
                    ""animation"":""background-happy-birthday"",
                    ""bg_enabled"":""0""
                 }
              },
              { 
                 ""word_id"":5,
                 ""title"":""Merry Christmas"",
                 ""color"":""#a1361f"",
                 ""background_color"":""#FFFFFF"",
                 ""style"":""normal"",
                 ""params"":{ 
                    ""animation"":""background-merry-christmas"",
                    ""bg_enabled"":""0""
                 }
              },
              { 
                 ""word_id"":6,
                 ""title"":""Congratulations"",
                 ""color"":""#0fd159"",
                 ""background_color"":""#FFFFFF"",
                 ""style"":""normal"",
                 ""params"":{ 
                    ""animation"":""background-congratulations"",
                    ""bg_enabled"":""0""
                 }
              },
              { 
                 ""word_id"":7,
                 ""title"":""Happy Easter"",
                 ""color"":""#0bb0b3"",
                 ""background_color"":""#FFFFFF"",
                 ""style"":""normal"",
                 ""params"":{ 
                    ""animation"":""background-happy-easter"",
                    ""bg_enabled"":""0""
                 }
              },
              { 
                 ""word_id"":8,
                 ""title"":""Happy Thanksgiving"",
                 ""color"":""#b5aa09"",
                 ""background_color"":""#FFFFFF"",
                 ""style"":""normal"",
                 ""params"":{ 
                    ""animation"":""background-happy-thanksgiving"",
                    ""bg_enabled"":""0""
                 }
              }
           ],
           ""on_thisDay"":""""
        }";
        }
    }
}