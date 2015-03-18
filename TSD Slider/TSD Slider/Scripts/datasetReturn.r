
#data set returns from sliderDataToComptuer
datasetwithseries <- function(dataset)
{
  
  #start a brand new dataset
  myset <- NULL
  
  for(i in seq(2,ncol(dataset),2))
  {
    #populating datasets
    myset <- rbind(myset,data.frame(cycles=(10000*(i/2)),displacement=1000*dataset[,i],force=dataset[,(i+1)]))
  }
  myset


  
  
}