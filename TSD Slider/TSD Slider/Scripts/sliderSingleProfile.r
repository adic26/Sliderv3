sliderdataSingleProfile <- function(directory,myrows = 1:2580) {
  ## 'directory' is a character vector of length 1 indicating
  ## the location of the CSV files
  
  #   print(directory,quote=TRUE)
  #   print(id,quote=TRUE)
  #   print(typeof(id),quote=TRUE)
  files_full <- list.files(directory,full.names=TRUE)
  retData <- data.frame()
  ## retData <- cbind(myrows)
  ## Return a data frame of the form:
  ## id nobs
  ## 1  117
  ## 2  1041
  ## ...
  ## where 'id' is the monitor ID number and 'nobs' is the
  ## number of complete cases  

    this_set <- read.csv(files_full,skip=4,sep="")
    mynewrow <- 1:nrow(this_set)
    retData <- cbind(mynewrow)
    displacement <- this_set$Py.m.
    force <- this_set$Fy.N.
    retData <- cbind(retData,displacement)
    retData <- cbind(retData,force)
  
  
  retData
  
}