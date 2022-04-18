using System.Collections.Generic;
using System.IO;

namespace WinUI.UseLiteDB.Models
{
    public class PersonalInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 头像名称
        /// </summary>
        public string AvatarName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 兴趣爱好
        /// </summary>
        public string Hobbies { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 头像流
        /// </summary>
        public Stream AvatarStream { get; set; }
    }

    public class PersonalInfoModel
    {
        public List<PersonalInfo> Data { get; set; }
    }
}
