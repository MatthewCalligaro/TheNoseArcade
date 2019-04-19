'''
This module allows a user to label all of the images in all of the directories
in a given dataset.
'''

import os
import cv2

X_COOR = 0
Y_COOR = 0

def get_points(event, x, y, flags, param):
    '''
    Mouse callback function that sets the value of global variables
    X and Y to the location of the mouse when it is clicked
    '''
    global X_COOR
    global Y_COOR

    # When the mouse is clicked, set the location
    if event == cv2.EVENT_LBUTTONDOWN:
        print(x, y)
        X_COOR = x
        Y_COOR = y

def get_coors(img_fn):
    '''
    Display an image, and after the user has selected a location in the image,
    write it to a file
    '''
    # Open the image
    img = cv2.imread(img_fn)

    # Set up to read from the mouse
    cv2.namedWindow('image')
    cv2.setMouseCallback('image', get_points)

    # Display the image
    cv2.imshow('image', img)
    cv2.waitKey(0)

    # Write the result to a file
    coor_file = open(img_fn[:-4] + '.txt', 'w')
    coor_file.write(str(X_COOR) + '\n' + str(Y_COOR))
    coor_file.close()

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
    coors_file = open(img_fn[:-4] + '.txt', 'r')
    x_coor = int(coors_file.readline())
    y_coor = int(coors_file.readline())
    coors_file.close()

    # Add the point
    cv2.circle(img, (x_coor, y_coor), 3, (0, 0, 255), -1)

    # Display the image
    cv2.imshow('image', img)

    cv2.waitKey(0)

    # Destroy the window
    cv2.destroyAllWindows()

def process_dir(img_dir, show_comp):
    '''
    Process a directory, either by labeling it or displaying the labels
    '''
    img_fns = os.listdir(img_dir)
    for img_fn in img_fns:
        if img_fn[-4:] == '.jpg':
            if show_comp:
                draw_labels(img_dir+img_fn)
            else:
                get_coors(img_dir+img_fn)

def main():
    '''
    For every directory in the path, if it has been labeled show labels otherwise
    allow the user to set them
    '''
    # Set up paths
    top_dir = '/Users/nandeekanayak/Downloads/gt_db/'
    done_dirs = ['s05', 's02', 's34', 's33', 's32', 's35', 's03', 's04', 's50', 's44',
                 's43', 's26', 's19', 's21', 's17', 's28', 's10', 's42', 's45', 's11',
                 's16', 's29', 's20', 's27', 's18', 's01', 's39', 's06', 's30', 's08',
                 's37', 's09', 's36', 's31', 's38', 's07', 's22', 's25', 's13', 's14',
                 's40', 's47', 's49', 's15', 's12', 's24', 's23', 's48', 's46', 's41']

    # Check if the user wants to skip the above directories
    show_comp = input('Show completed?') == 'yes'

    img_dirs = os.listdir(top_dir)
    for img_dir in img_dirs:
        print(img_dir)
        if len(os.path.split(img_dir)[1]) == 3 and \
            ((show_comp and img_dir in done_dirs) or (not show_comp and img_dir not in done_dirs)):
            process_dir(top_dir + img_dir + '/', show_comp)
            cont = input("Continue?")
            if cont != 'yes':
                break

if __name__ == '__main__':
    main()
    