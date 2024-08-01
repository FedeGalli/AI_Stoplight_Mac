# Stoplight Reinforcement-Learning Optimizer

This Unity Project has the idea to test how to optimeze the wait-time of cars waiting at Stoplights intersections. <br />

To do so, i recreated using Unity the traffic flow logic of single cars arriving at an intersection. While, by using a Reinforcement learning algorith (PPO) applied to all the stoplights, i managed the green/red timings based on the traffic arrival at each road.<br /> The main goal of the algorithm maximise the reward gained by letting pass the most number of cars it can. <br />

The reward is not only based of the number of cars traversing the intersection, but i have a general function wich take the other cars wait-time on the other roads. <br />
That's because the main focus is to let the car wait the less as possible at the intersection, imagine having an infinite traffic jam just on one road, if we only take into account the # of cars, the lights just stay green forever on that direction, making the other roads wait for an infinite amount of time. This is the reason we need to know for how log the Green Lights as been up in a certain direction.
