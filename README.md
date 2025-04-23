# Investigating the Landscape of Machine Learning in Anti-Cheat Software

### A Undergraduate Honors Thesis for Arizona State University.

The perfect anti-cheat software for a first person shooter that balances protecting user
privacy and effective cheat detection in a modern age where dishonest methods of gameplay are
rampant within competitive games. By utilizing the inherent protections servers have against
third party attacks, by removing the software off of the client, all of the detection methods are
placed in an external area, where cheaters are determined by behavior that is tracked through
statistical trackers placed in the game. By measuring multiple key features including Illegal
Trace Time, Trigger Time, and Mouse Flick Speed. Each of these measured attributes relate to
commonly used cheats in first person shooters, which is the target for this anti-cheat machine
learning model. By gathering a wide range of statistics and figuring out the average player’s
statistics, it would be possible to determine if a player is using external programs to gain an
unfair advantage.

### Bland-FPS
The first person shooter created in Unity that supports host-client multiplayer. Contains basic
features to mimic the core fundamental mechanics that exist in most first person shooters currently
on the market.

### Cheats
The cheats are integrated into the game's files to bypass the need for dll injection every time
the game is launched for ease of use. These cheats include ESP, a trigger bot, and an aimbot.

### Anti-Cheat
The anti-cheat software in the game tracks three statistics: Illegal Trace Time, Trigger Time, and
Flick Speed. Each of these features are targeted to detect each of the cheating behaviors that the
implemented cheats provide the player. This data is tracked locally into the user's computer, but
realistically would be sent from the game to the server, outside the reach of the client's influence.
This data is then fed into the Model to determine whether or not that tuple of data indicates whether
or not that player is behaving like a cheater.

### Data Pre-Processing: Cheat Score
The 3 statistics tracked are weighted differently from one another, and fed into an equation that creates
another statistic called the Cheat Score, which is a metric that determines how likely a player is cheating.
A high cheat score indicates that a player is cheating while a low or average cheat score indicates
below average or average play. 

### The Model
The machine learning model used was a relatively simple one, K-Means Clustering, to group together different
types of Cheat Scores and classify the data into 4 clusters: Poor Players, Average Players, Good Players,
and Cheaters. Each set of data that is fed into the model retrains the model and creates new clusters. This
means the model becomes better at classifying cheaters with more datapoints
