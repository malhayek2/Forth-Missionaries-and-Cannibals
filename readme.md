# The Problem
Suppose there are three missionaries and three cannibals who need to cross to the far side of a river using a single boat that can carry one or two people at a time. Both groups will cooperate and can paddle back and forth freely, but old habits will lead the cannibals to eat the missionaries if the missionaries are ever outnumbered on either side of the river.

**The problem** is to find a way to get all of the missionaries and all of the cannibals safely across the river.
##### Data structures
Store three values on the stack for a single state, near or far, missionaries, cannibals in a packed format, start state  [2 3 3 ] true 3 3 
There will be 3 stacks: used stack, candidate stack and crumb stack.

## Sudo Code
- push the start state on the candidate stack
- search:
- - print the candidate stack
- - pop a candidate state off the candidate stack
- - push a copy on the bread-crumb trail stack
- - if it is the goal state
- - - print out the contents of the bread-crumb trail in order. this is the solution to the puzzle.
- - else
- - - generate a list of successor states (there should be exactly 5)
- - -  push the valid, legal, fresh successors on the candidate stack
- -  - for each successor generated in this step:
- - -  - call search recursively
- - - pop the state off the bread-crumb trail stack

### Installation

This application requires gforth and git.

Install the dependencies in Ubuntu just like this
```
sudo apt install gforth
sudo apt install git
```
## How to Run
```
gforth ./missionaries.fs
start
bye
```


### Development
Done


### Todos

 - submit it

License
----
**Free Software i guess**


