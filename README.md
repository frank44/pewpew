# GENERAL INFORMATION

Eve's Actions
Move Left  = A or Left arrow
Move Right = D or Right arrow
Jump       = Q or Up arrow
Dash       = Space
Crawl      = S or Down arrow

Note: You cannot dash while crawling, but a dash can turn into a crawl.

# Standing bugs
:: You die if you hit the side of the screen with too much velocity. (Rare)
:: UPDATE: I haven't seen this happen anymore, it might be a relic caused by dashing too quickly. More testing is needed.
:: You can end up in the middle of obstacles if you stop dashing or crawling while underneath them. 
:: When you duck the animation seems to go down slighly. Might be fixed when we update the sprites, but I'll leave this here till then.
:: Crawling under an obstacle from the right side from a stand still is impossible for some unknown reason when you start out RIGHT NEXT to the obstacle.
  
----------------------------------------------------------------------------

# Frank's Log

2/10/12
:: Implemented Dashing

2/11/12 
:: BUGFIX - Dying animation is now animating correctly when you die during a dash 
:: BUGFIX - Jumping under an obstacle used to leave you suspended for an instant, this had been greatly reduced
	     
2/15/12
:: Dashing now modifies your hitbox, can be used to slide under obstacles
:: Crawling implemented, it's hitbox is the same as dashing. The stading animation is used for now.
:: Dashing animation is now different from running (the celebrate animation will be used as a place holder for now)

2/16/12
:: BUGFIX - Crawling to the left now works. Also crawling speed is increased. 
              
# Goals

- Implement shooting for Eve
- Implement mob AIs
- Give the player the ability to aim Eve's shots              
              
----------------------------------------------------------------------------

# Andy's Log

2/19/12
:: Completed scrolling of background alongside screen both vertically and hoizontally

2/14/12
:: Completed side scrolling both vertically and horizontally for any screen size and level size 

# Goals:

- Factoids upon death
- Pause screen and menus

----------------------------------------------------------------------------

# Oscar's Goals:
:: Stop being so childish
