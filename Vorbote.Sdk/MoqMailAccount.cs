using System.Collections.Generic;

namespace Vorbote
{
    public class MoqMailAccount
    {
        public string Id { get; set; }

        public string AccountId { get; set; }

        public string DisplayName { get; set; }

        public IEnumerable<string> Users { get; set; }
    }
}