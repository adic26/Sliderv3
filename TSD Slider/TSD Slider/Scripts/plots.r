plot_lines_2 <- function(cleandata)
{
  library(ggplot2)
  #,group=cycles
  sp <- ggplot(data=cleandata,aes(x=disp,y=force)) +
    #geom_smooth(se=FALSE) +
    geom_line(aes(colour=factor(cycles))) +
    #geom_smooth(aes(colour=factor(cycles))) +
    #geom_point(aes(colour=factor(cycles))) +
    scale_colour_discrete() +
    #scale_colour_hue("clarity") +
    #scale_colour_brewer(palette="Set1") +
    #scale_color_brewer(palette="YlOrRd") +
    #scale_colour_gradient(low="red") + #continuous scale
    #geom_density(alpha=.3) +
    ggtitle("Force Vs. Displacement") +
    xlab("Displacement [mm]") +
    ylab("Force [Newtons]")
  sp
  
}

plot_density <- function(cleandata)
{
  
  library(ggplot2)
#   sp <- ggplot(data=cleandata,aes(x=disp,y=force)) +
#     geom_point(shape=1,aes(colour=series),fill="red") +
#     #geom_line() +
#     #geom_density(alpha=.3) +    #only seems to work with histograms
#     ggtitle("Force Vs. Displacement") +
#     xlab("Displacement [mm]") +
#     ylab("Force [Newtons]")

  #sp <- ggplot(data=cleandata,aes(x=disp,y=force,colour=series)) +
sp <- ggplot(data=cleandata,aes(x=disp,y=force)) +
  geom_line(aes(colour=cycles)) + #continous data
  #scale_colour_gradientn(colours=rainbow(2))
  #scale_colour_gradientn(colours=heat.colors(4,alpha=0.6))
  #scale_color_brewer(palette="YlOrRd") #discrete scale
  #scale_colour_gradientn(colours=topo.colors(10))
  scale_color_gradient(high="red") #continuous scale
  
  #sp + scale_colour_discrete(name="Characterization Every 10K cycles")
  sp
  
}

plot_lines <- function(cleandata)
{
  
  library(ggplot2)
  qplot(disp, force, group=cycles, data=cleandata, col=factor(cycles), geom=c("point","smooth"),main="Force Vs. Displacement")
}

#Heat map using Hex -- still in development
plot_hex <- function(cleandata)
{
 library(ggplot2)
 library(hexbin)
 sp <- ggplot(data=cleandata,aes(x=disp,y=force)) +
   #geom_hex(aes(color=factor(cycles))) +
   geom_hex() +
   scale_fill_gradientn(colours=rainbow(7))
 
 sp
   
   
}