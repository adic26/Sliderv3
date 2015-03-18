characterization_lines <- function(directory)
{
  #source('M:/Fanuc & Slider/sliderDataToComputer.r')
  #source('M:/Fanuc & Slider/datasetReturn.r')
  #source('M:/Fanuc & Slider/plots.r')
  
  oldset <- sliderdata_yprofile(directory)
  cleanset <- datasetwithseries(oldset)
  plot_lines_2(cleanset)

}

characterization <- function(directory)
{
  #source('M:/Fanuc & Slider/sliderDataToComputer.r')
  #source('M:/Fanuc & Slider/datasetReturn.r')
  #source('M:/Fanuc & Slider/plots.r')
  
  oldset <- sliderdata_yprofile(directory)
  cleanset <- datasetwithseries(oldset)
  plot_lines(cleanset)

}