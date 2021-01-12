# School timetable through genetic algorithms.

Team members: @SabaAlex and @RusuRaul

## Goal

  Generate a correct school timetable through a genetic algorithm

## Requirement

  Each project will have 2 implementations: one with "regular" threads or tasks/futures, and one distributed (possibly, but not required, using MPI). A third implementation, using OpenCL or CUDA, can be made for a bonus.

  The documentation will describe:

    the algorithms,
    the synchronization used in the parallelized variants,
    the performance measurements

## Computer Specification

* CPU: Intel Core i7-7700HQ, 2.80GHz
* RAM: 16 GB

## Algorithm

  In the following section, we will present a brief description.
  The first step is to init our Population.
  After that we start our survival process throughout a number of generations

## Algorithm details description

Gene: (Tuple/Felie)
    Time: 0-9
    Location: 0-L
    Day: 0-5

    L - Given number of Locations

Individual:
    N * Gene
    N - GroupsNumber * CoursesNumber

    GroupsNumber - Given number of groups
    CoursesNumber - Given number of courses

Population:
	M Individual

Selection:
	Tournament
	M / 2 samples
	Crossover between the 2 best fit

Crossover:
	Gena -> Atribute -> .5  Chance for the fatherAttribute to end up in the 1st kid, otherwise the motherAttribute
    2 Kids per crossover

Mutation:
	Individual -> Gena -> Atribut -> Mutate Gene (dam o valoare random din intervalul atributului) cu o prob mica
	
Fitness:
	Best : 0
	Individual -> Gene -> How many Genes are distinct


#### Problem


## Short Description of the Implementation:

* Threads
* Distributed - MPI


### Threads

    Parallelized selection, all samples are computed at the same time
    Parallelized fitness computation
    Parallelized crossover between individuals, each gene is computed in paralel for the 2 next individuals
    Parallelized mutation

### Distributed - MPI


## Performance Tests

| Algorithm                        | Population: 100, SampleSize: 10, Generations: 20 | Population: 20, SampleSize: 5, Generations: 10 | Population: 50, SampleSize: 5, Generations: 10 |
| -------------------------------- |:--------:|:-------:|:---------:|
| Threads         | 652,200 ms |  15000 ms | 120000 ms |
| Distributed MPI | ? ms | ? ms | ? ms |


## Conclusion