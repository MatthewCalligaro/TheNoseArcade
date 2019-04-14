import cv2

import os

X = 0
Y = 0

def get_points(event,x,y,flags,param):
    '''
    Mouse callback function that sets the value of global variables 
    X and Y to the location of the mouse when it is clicked
    '''
    global X
    global Y

    # When the mouse is clicked, set the location
    if event == cv2.EVENT_LBUTTONDOWN:
        print(x, y)
        X = x
        Y = y

def get_coors(img_fn):
    '''
    Display an image, and after the user has selected a location in the image,
    write it to a file
    '''
    global X
    global Y

    # Open the image
    img = cv2.imread(img_fn)

    # Set up to read from the mouse
    cv2.namedWindow('image')
    cv2.setMouseCallback('image',get_points)

    # Display the image
    cv2.imshow('image',img)
    cv2.waitKey(0)

    # Write the result to a file
    f = open(img_fn[:-4] + '.txt', 'w')
    f.write(str(X) + '\n' + str(Y))
    f.close()

    # Destroy the window
    cv2.destroyAllWindows()

def draw_labels(img_fn):
    '''
    Given an image, display the image with the nose labeled
    '''
    # Open the image
    img = cv2.imread(img_fn)

    # Set up to read from the mouse
    cv2.namedWindow('image')

    # Read in the point
    f = open(img_fn[:-4] + '.txt', 'r')
    x = int(f.readline())
    y = int(f.readline())
    f.close()

    # Add the point
    cv2.circle(img,(x,y), 3, (0,0,255), -1)

    # Display the image
    cv2.imshow('image',img)

    cv2.waitKey(0)

    # Destroy the window
    cv2.destroyAllWindows()

def process_dir(img_dir, show_comp):
    '''
    Process a directory, either by labeling it or displaying the labels
    '''
    img_fns = os.listdir(img_dir)
    for fn in img_fns:
        if fn[-4:] == '.jpg':
            if show_comp:
                draw_labels(img_dir+fn)
            else:
                get_coors(img_dir+fn)

def main():
    '''
    For every directory in the path, if it has been labeled show labels otherwise allow the user to set them
    '''
    # Set up paths
    top_dir = '/Users/nandeekanayak/Downloads/gt_db/'
    done_dirs = ['s05', 's02', 's34', 's33', 's32', 's35', 's03', 's04', 's50', 
        's44', 's43', 's26', 's19', 's21', 's17', 's28', 's10', 's42', 's45',
        's11', 's16', 's29', 's20', 's27', 's18', 's01', 's39', 's06', 's30',
        's08', 's37', 's09', 's36', 's31', 's38', 's07', 's22', 's25', 's13',
        's14', 's40', 's47', 's49', 's15', 's12', 's24', 's23', 's48', 's46',
        's41']

    # Check if the user wants to skip the above directories
    show_comp = input('Show completed?') == 'yes'

    img_dirs = os.listdir(top_dir)
    for dir in img_dirs:
        print(dir)
        if len(os.path.split(dir)[1]) == 3 and \
            ((show_comp and dir in done_dirs) or (not show_comp and dir not in done_dirs)):
            process_dir(top_dir + dir + '/', show_comp)
            cont = input("Continue?")
            if cont != 'yes':
                break

if __name__ == '__main__':
    main()
    