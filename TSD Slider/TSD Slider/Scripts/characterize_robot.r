characterization_robot <- function(directory)
{
  #source('C:/Users/achugh/Documents/Graphs/sliderDataToComputer.r')
  #source('C:/Users/achugh/Documents/Graphs/datasetReturn.r')
  #source('sliderDataToComputer.r')
  #source('datasetReturn.r')
  
  #oldset <- sliderdata_yprofile(directory)
  oldset <- sliderdataSingleProfile(directory)
  cleanset <- datasetwithseries(oldset)
}