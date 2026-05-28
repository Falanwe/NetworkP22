using System;
using System.Collections.Generic;
using System.Text;

namespace FileServer
{
    internal class FileManager
    {
        private readonly Dictionary<string, (string name, byte[] content)> _files = [];

        public int GetMemoryUseForUser(string userId) => _files.Values.Where(f => f.name == userId).Sum(f => f.content.Length);
        public void AddFile(string userId, string fileName, byte[] content)
        {
            _files[fileName] = (userId, content);
        }
        public bool TryGetFile(string fileName, out byte[] content)
        {
            var result = _files.TryGetValue(fileName, out var entry);
            content = entry.content;
            return result;
        }
    }
}
