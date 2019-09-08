( helper functions )

: 3dup ( x y z -- x y z x y z)
   dup
   2over
   rot 
;

: 3drop ( x y z -- )
   drop
   2drop 
;

: pack ( near m c -- packedstate ) 
   swap
   10 * +
   swap
   100 * + 
;

: unpack ( packedstate -- near m c )
   dup
   10 mod  
   swap    
   dup
   100 /   
   swap    
   100 mod 
   10 /    
   rot     
;

: printstate ( side m c -- )
  swap
    rot
  ." [ " 2 = if ." near " else ." far  " then . . ." ]"

;   

( Variables )
variable used 100 cells allot
variable usedcounter
variable candidate 100 cells allot
variable candidatecounter
variable crumb 100 cells allot
variable crumbcounter 

\ test if n is in the used set
: isused ( n -- bool )
    \ assume false result
    0            ( n false )
    swap            ( false n )
    \ loop through all the elements
    usedcounter         ( false n &usedcounter ) 
    @            ( false n usedcounter )
    0            ( false n usedcounter 0 )
    do            ( false n )
        \ compare n with elt i
        dup        ( false n n )
        used        ( false n n &used )
        i        ( false n n &used i )
        cells        ( false n n &used i *8 )
        +        ( false n n &used[i] )
        @        ( false n n used[i] )
        =        ( false n n==used[i] )
        if        ( false n )
          nip        ( n )
          -1        ( n true )
          swap        ( true n )
          leave    ( true n )
        then
    loop            ( bool n )
    drop            ( bool )
;

\ add n to the used set
: addused ( n -- )
  used usedcounter @ cells + ! ( stores )
  1 usedcounter +!
;

\  push a value on the candidate stack
: pushcandidate ( n -- )
    candidate candidatecounter @ cells + !
  1 candidatecounter +!
;




\ pop a value off the candidate stack
: popcandidate ( -- n )
  -1 candidatecounter +!
  candidate candidatecounter @ cells + @
;

\ push a value on the bread crumb trail stack
: pushcrumb ( n -- )
    crumb crumbcounter @ cells + !
  1 crumbcounter +!
;

\ pop a value off the bread crumb trail stack
: popcrumb ( -- n )
  -1 crumbcounter +! 
  crumb crumbcounter @ cells + @
;




( Debugging )
\ helper function
: dump ( addr cell-count -- )
  0 do cr dup i cells + @ unpack printstate  loop drop
;

\ print the contents of the used set in order
: printused 
  usedcounter @ 0 = if ." empty "
  else used usedcounter @ dump
  then
  
;

\ print the contents of the candidate stack in order
: printcandidates ( -- )
  candidatecounter @ 0 = if ." empty "
  else cr ." Candidates : " candidate candidatecounter @ dump
  then
;


\ print the contents of the bread crumb trail in order
: printcrumbs 
  crumbcounter @ 0 = if ." empty "
  else crumb crumbcounter @ dump
  then
;



\ push the starting state onto the stack true 3 3
: startstate ( -- near m c )
   2 3 3
;

\ test if the state on the stack is the goal state
: isgoal ( near m c -- bool )
    pack
    300 =
;

\ test if the state on the stack is valid and legal
: isvalid ( near m c -- bool )
  ROT		( m c side)
  DROP		( m c )
  DUP 0 >=      ( m c c ) \ is c > 0 ? 
  SWAP		( m bool c )
  DUP 3 <=	( m bool c bool ) \ is c <= 3?
  ROT		( m c bool bool )
  AND		( m c bool )

  ROT		( c bool m )
  DUP 0 >=	( c bool m bool )
  SWAP		( c bool bool m )
  DUP 3 <=	( c bool bool m bool )
  ROT		( c bool m bool bool )
  AND		( c bool m bool )

  ROT		( c m bool bool )
  AND		( c m bool ) 
  \ m == c
  -ROT ( bool c m )
  DUP ( bool c m m )
  ROT ( Bool m m c )  
  =   ( bool m bool )
  \ m is 0
  SWAP ( bool bool m )
  DUP	( bool bool m m )
  0=    ( bool bool m bool )
  
  SWAP ( bool bool bool m )
  3 =  ( bool bool bool bool ) 
  
  OR
  OR ( bool bool ) 

  AND
;

\ add a state to the candidate stack if it is valid and new
\ report on the outcome: invalid, repeat, or fresh
: addcandidate ( near m c -- )
  3dup
  isvalid invert if cr  ." invalid " printstate 
  else
  3dup pack isused if cr ." repeat  " printstate
  else
  3dup pack dup addused pushcandidate cr ." fresh   " printstate
  then
  then 
; 
\ find all successor candidates for the given state, push them on stack
\ leaves the number of states added on the stack
: successors ( near m c -- )
\ all near
    rot dup
    2 = 
    if
    rot rot 
\ cannibal far
    3dup
    1 -
    rot 1 + rot rot
    addcandidate

\ two cannibals far
    3dup
    2 -
    rot 1 + rot rot
    addcandidate

\ missisonary far
    3dup
    swap 1 - swap
    rot 1 + rot rot
    addcandidate

\ two missinoaries far
    3dup
    swap 2 - swap
    rot 1 + rot rot
    addcandidate

\ each far
  3dup
    1 -
    swap 1 - swap
    rot 1 + rot rot
    addcandidate
\ then all far
    else
    rot rot   

\ missionary near
    3dup
    swap 1 + swap
     rot 1 - rot rot
    addcandidate

\ cannibal near
    3dup
    1 +
    rot 1 - rot rot
    addcandidate

\ two missionaries near
    3dup
    swap 2 + swap
    rot 1 - rot rot
    addcandidate

\ two cannibals near
    3dup
    2 +
    rot 1 - rot rot
    addcandidate

\ each near
    3dup
    1 +
    swap 1 + swap
    rot 1 - rot rot
    addcandidate
    then 3drop
;




\ find the solution from position at top of stack
: search ( -- )
  printcandidates
  popcandidate
  dup
  pushcrumb   
  unpack
  3dup
  isgoal if cr cr ." solution found " cr
  printcrumbs
  3drop
  else
  \ drop
  \ cr ." Candidates:"
  successors  ( bool m c bool )
  cr
  recurse 
  popcrumb
  then
; 
\ start
 : start ( -- )
  0 usedcounter !
  0 crumbcounter !
  0 candidatecounter !
  startstate
  pack
  dup
  pushcandidate
  addused
  search
;

