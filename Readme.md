# BluePrism WordPuzzle challenge


### My background

Since I don't have a computer science background, my knowledge on algorithms
(and Big O notation which is quite important to optimally solve this word puzzle) is quite limited.


### Figuring out the algorithm

Despite that, I tried first to come up with some ideas... without much success.

(this is when I started questioning my life choices :D)

I then tried to gather some ideas online.

Saw [someone](https://stackoverflow.com/a/9283294/2856472) suggesting to change character by character until
we found the End word.
I tried to write down some code, didn't seem like a viable solution.
It could have worked, but at the time it didn't make much sense in my head.

I also saw [someone](https://stackoverflow.com/a/2205549/2856472) mentioning creating a graph that would
have words (from the dictionary) as nodes and the edges would the 1 character difference.
I never studied graphs in school so I wasn't very comfortable with the matter.
I never had to use graphs professionally, so although I understood the idea, I couldn't really figure out how to execute it in C#.

(still questioning my life choices and thinking if I should change career :D)


### The solution

If I would begin from the Start word it would be easy to find the words that were one character different, the problem would 
be to find the one that was closer to the End word (instead of just any next word).

So I started wondering... If I can't figure out how to do this for any number of characters let's try to do exactly
what they ask and find a solution for 4 characters words.

That got me thinking... maybe I can get a list of words that are 1 character different from the Start word and another list
of words that are 1 character different from the End word.

If 'lucky' I would find the solution right away, by intersecting the 2 lists.
EX.:
words: hide - sore
resulting sequence: hide - hire - sire - sore

If not, then I would have to do find the 'middle' word (sire in the example below).
EX.:
words: hide - sort
resulting sequence: hide - hire - sire - sore - sort

Started writing down some code and although this did seem like a terrible solution, it seemed like it could work.


### Nuget packages

1. [Cocona](https://github.com/mayuki/Cocona) make Console Application setup and argument validation easier
2. [Ardalis.GuardClauses](https://github.com/ardalis/guardclauses) to declutter parameter checking
3. [FluentValidation](https://fluentvalidation.net/) to simplify argument validation
4. [NSubstitute](https://nsubstitute.github.io/) instead of Moq (personal preference)
5. [Serilog-Sinks-TestCorrelator](https://github.com/MitchBodmer/serilog-sinks-testcorrelator) to facilitate log testing


### Performance considerations

1. Used 'for' loops instead of 'foreach' as a performance consideration
2. Validated console parameters in constructor to avoid any further logic
3. Checked if Start and End words were the same before loading the dictionary (to prevent it if possible)
4. In general doing so many cycling through lists is not optimal, but it was the solution I could find for this problem


### Notes

1. To respect the first of the SOLID principles, opening and reading the text from the dictionary file was handled 
separately from the word puzzle logic.
2. Tried to follow TDD (although not used to doing it)
3. Could have setup logging with Serilog, but seemed a bit overkill for this solution
