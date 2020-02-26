from PIL import Image
import glob

for name in glob.glob('*.png'):
    im = Image.open(name)
    name = str(name).rstrip(".png")
    im = im.convert(mode='L')
    im.save(name + '.jpg')
    break

for name in glob.glob('*.tiff'):
    im = Image.open(name)
    name = str(name).rstrip(".tiff")
    im.save(name + '.jpg', 'JPEG')

print ("Conversion from tif/tiff to jpg completed!")
