Team Members:  Connor Gerrits, James Elliott, Bryan Song

Features made in Milestone 1:

TO RUN:
Option A) Open up Unity and select the ForethoughtAI Folder,
open up test-scene-1.unity file.  Hit the play button at the top of the editor
to play
Option B)  Open up the Builds folder within ForethoughtAI, and run the .exe

Features:
1.  Functioning Board Display:  Game is properly initialized and rendered to a
  5x5 board.  Each empty space is depicted as a black space.  Cards that are face
  up display the correct card value, and face down cards are properly face down
2. Functioning Hand:  Properly renders and displays the player's hand.  Each time
  a player plays a card from their hand, a new card is automatically added(per the
  game's rules).
3. Functioning Selecting of Cards:  Player can properly select cards on the
  board and in their hand.  To do so,  A player needs to just click on either a
  board space that contains a card, or a card in their hand. The outline will
  change to red to indicate selection.  Players can click on a different card to
  change selection, and freely select between cards on board and cards on hand.
  Additionally, clicking the selected card will automatically deselect the card.
4. Play Cards:  The game has 3 out of the 4 actions a player can take on their
turn implemented within unity (the 4th is in the model but not hooked up).
    a) Play card From Hand:  Player can select a card in their hand, and then
       click on an empty space. That card will be played from hand onto the Board
       (maintaining face up or face down properly).
    b) Peek at a facedown card: Player may click on a face down board space and
       select the "Peek" button.  Doing so will flip the card over for a few
       seconds before flipping it back over.
    c) Swap 2 Cards on the Board:  Players may select a card on the board, click
       the swap button, then select another card on the board.  To swap, both
       cards must be either both face up or both face down (not 1 of each). If
       valid, the swap will happen, else nothing will occur
5.  Buttons for board spaces:  As noted above, when selecting a space the
appropriate action buttons are displayed. If it's a face up card, peek is not
available, for example.

What we DIDN'T get to this Milestone:
1)  Game Over:  Implemented in model, not yet hookup up in unity
2)  Player Turn management:  Did not get to formally create turn manager within
  this milestone.
3)  AI implementation:  was not in scope for this milestone, but did start
  organizing model requirements for AI, and how to organize MCTS for this assignment
