# unit-of-work

Generic Unit of Work implementation taken 
	from https://coding.abel.nu/2012/10/make-the-dbcontext-ambient-with-unitofworkscope/ 
	taken one step further to allow for scopes to use with more types outside DbContext.

To Keep with IoC, IUnitofWorkFactory can be changed to suit individual needs for creating
	the IUnitofWork instances.