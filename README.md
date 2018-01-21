# demopadr
demopadr.exe is a tool by anotak for padding demos for vanilla compatibility during the demo reel when
you start doom. this is due to the revenant tracer desync issue.
for more information see this [DoomWiki article](https://doomwiki.org/wiki/Revenant_tracers_desync_internal_demos).
demopadr is a commandline tool that runs on demo lmp files.
correct usage is:
      demopadr <input> <output>

for this to work, all 3 demos in a wad must be padded. otherwise demos with revenants may still desync on vanilla/choco doom.

demopadr.exe requires .NET 3.5 or Mono. 3.5 is included in Windows 7 or above. If you're on Linux or OSX, it should work with Mono.

# license
MIT license:
Copyright 2018 anotak

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
