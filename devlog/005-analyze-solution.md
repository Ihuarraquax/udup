﻿Now, goal is to create file with all required data to build living documentation at runtime

I'm digging into roslyn and how to efficiently extract data from code. I'm looking for all helpers to do that, to not get stuck with fighting edge cases with manual traversing.

I've found SymbolFinder class, which is full of helpful methods to find where given symbol is used

I will use opensource tool for analyzing solution and generate output file with all classes described, instead of writing it for scratch.

I've decided to use https://github.com/eNeRGy164/LivingDocumentation to analyze source code.
Goal for now will be to add result to dockerfile and analyze it at runtime based on fluent configuration.
For the start I will recreate same functionality that I did with reflection.
