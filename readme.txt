This interpreter can interpret NC programs written in G code (e.g, SINUMERIK 840Dsl, LinuxCNC specifications).

G code commands are divided into basic commands and specific commands. Here, basic commands are commands
whose expressions and meanings are similar or the same among different G code programming languages,
such as "G0\G00" often means rapid traverse. In contrast, specific commands refer to commands that are different
among different G code programming languages. 

Therefore, we label interpreters as basic interpreters and specific interpreters.

Edit by a new branch