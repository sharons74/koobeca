using System;

namespace KoobecaFeedController.DAL {
    [AttributeUsage(AttributeTargets.Property)]
    internal class MySqlIgnoreAttribute : Attribute { }
}