# CaptureTheFlagGeneticAlgorithm
Capture The Flag!

I’m determining how well a genetic algorithm can play ‘Capture The Flag’. Capture The Flag is a faceoff between two teams of equal size in a field. Each team also has a flag and a base. The objective of the game is to carry the opposing team’s flag to your base. This can be done by agents on a team picking up the flag and running over to the base.


(Red team vs. Blue Team)
(Orange Square = Red Base, Green Square = Blue Base, Turquoise square = Blue’s flag, Yellow square = Red’s flag). The red agents should work to get blue’s (turquoise) flag to the orange square)

Agents can prevent opposing agents from carrying a flag by tagging them, that is, colliding with them. In doing so, the rules establish that both agents who have collided must drop the flag on the ground if they are holding one, and return back to their respective home bases before being allowed to participate in the game once again. That is, until the agent has tagged back in, they are not allowed to tag others nor interact with any flags.

Constraints:
Agents must stay within bounds of the environment.
Agents are allowed to pick up both the enemy’s flag and their own as a defensive mechanism.
Every agent knows the positions of all other agents, flags, and bases.
Each instantiation of the game has different agent initial positions, speeds, base positions, and flag positions.

My approach:

To simulate the rules of this game, I utilized the Unity Game Engine to create a visual simulation of playouts of Capture The Flag.

Algorithmic Approach:

This is a complicated game requiring continuous real-time decision-making for which a tree search or similar algorithm used for turn-based games will not suffice. However, it is perfect information. 

My initial thoughts were to use a feed-forward or convolutional neural network, but I struggled to think about the most appropriate way to train the network: I would have required training data from the specific simulation that a) had not yet been built, and b) I couldn't synthesize samples for without initial player agents. The lack of prior examples put imitation learning out the window, unless I created a heuristic agent that I would coax the NN to defeat. Reinforcement learning was also an option, but it seemed limiting to carefully craft the numerical rewards for every step towards the desired action, when my only metric and true concern was whether a team won or lost the game.

This brought in my idea of genetic algorithms. What if every team attached a NN to its agents, and the team that was victorious allowed for the reproduction of its NN with slight possible mutations. After each game phase / episode, all that would need to be done is to select a winner and play out its children in future game phases.

Conclusion:
Overall I simplified this approach for the versus setting, where I had the winning agent create a child to play against in the next episode. Parallelizing that over 9 different playing fields allowed for different strategies to develop independently.

Testing:
Testing will be done against random agents, since there isn’t enough training time to create a very very strong genetic algorithm agent.
