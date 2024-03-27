Analyzing https://github.com/eNeRGy164/LivingDocumentation repository

takeaways:
 - this tool analyzes codebase and saves result to a json file. This file contains all metadata about code, a lot more that you could get from reflection. This result is a base for later documentation. 
 - It assumes that every project is different and it needs to generate documentation from this result file. This is a good approach to split gathering data to build phase and analyze it at runtime with easy configuration as a code.

I will do the same. First, incrementally gather data from code that later will be processed to generate documentation based on per project configuration.