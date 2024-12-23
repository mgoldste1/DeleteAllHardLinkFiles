# DeleteAllHardLinkFiles
Little utility to send all copies of a file with hard links to the recycling bin. integrated into windows explorer. Pairs nicely with Link Shell Extension (https://schinagl.priv.at/nt/hardlinkshellext/hardlinkshellext.html)


Step 1: Install Ln with Choco using the command "choco install ln". Other ways to install it and more details about this are at https://schinagl.priv.at/nt/ln/ln.html

Step 2: Make sure you have .net 8 runtime installed.

Step 3: Drop a SHORTCUT to the exe in shell:sendto  (C:\Users\[username]\AppData\Roaming\Microsoft\Windows\SendTo)

Step 4: Restart explorer (might not be necessary)

Now if you want to delete a file with 20 hard links, right click one of them, got to the send to menu, and click the DeleteAllHardLinks option. Check your recycling bin after that and you should see them all there.
