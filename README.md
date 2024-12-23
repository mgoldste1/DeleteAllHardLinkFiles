# DeleteAllHardLinkFiles
Little utility to send all copies of a file with hard links to the recycling bin. integrated into windows explorer. Pairs nicely with Link Shell Extension (https://schinagl.priv.at/nt/hardlinkshellext/hardlinkshellext.html)

![hard link delete](https://github.com/user-attachments/assets/08423faf-c69d-4474-945d-3f404141f4f2)


Step 1: Install Ln with Choco using the command "choco install ln". Other ways to install it and more details about this are at https://schinagl.priv.at/nt/ln/ln.html

Step 2: Make sure you have .net 8 runtime installed.  (https://dotnet.microsoft.com/en-us/download/dotnet/8.0 - the ".net desktop runtime" one is on the right in the middle. I believe this one is what you want)

Step 3: Drop a SHORTCUT to the exe in shell:sendto  (C:\Users\[username]\AppData\Roaming\Microsoft\Windows\SendTo).  Whatever you name it is how it will show up in explorer.

Step 4: Restart explorer (might not be necessary)

Now if you want to delete a file with 20 hard links, right click one of them, go to the "send to" menu, and click the DeleteAllHardLinks option. Check your recycling bin after that and you should see them all there.

