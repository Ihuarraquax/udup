## How to display diagrams from parsed data?

(Q) Can I create UI that:
- will be easy to add to any project (like swagger)
- will allow to grab data from multiple instances and merge it
- will be interactive to show different perspectives of data

# Architecture
### Possible solutions:
#### a. Create separate frontend application that pull data from servers

#### b. Return UI page from server (e.g. swagger, nuke plan)

How its done in NUKE plan - static diagram with some some dynamic styling e.g. hovering node changes styles for it and it's connections. 
It wont be enough for showing different perspectives. Can use same diagram library for this solution.

How its done in Swagger - interactive UI served from server (allows http calls). Easy to append to every application regardless of technology stack


## Even interactive SPA app can be built and served from asp.net middleware like in swagger


Conclusion: I should create frontend in simple and lightweight stack. At first I will use anything that is easy and fast to prototype
